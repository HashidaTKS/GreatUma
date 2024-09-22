using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreatUma.Utils;
using GreatUma.Infrastructures;
using GreatUma.Models;
using GreatUma.Infrastructure;
using GreatUma.Model;

namespace GreatUma.Domain
{
    public class AutoPurchaserMainTask
    {
        public bool Running => CancellationTokenSource != null;

        private CancellationTokenSource CancellationTokenSource { get; set; }
        private CancellationToken CancelToken { get; set; }
        public TargetConfigRepository TargetConfigRepository { get; set; }
        private HashSet<RaceData> AlreadyPurchasedRaceHashSet { get; set; } = new HashSet<RaceData>();

        public void Run()
        {
            if (Running)
            {
                return;
            }
            LoggerWrapper.Info("Start AutoPurcaserMainTask");
            CancellationTokenSource = new CancellationTokenSource();
            CancelToken = CancellationTokenSource.Token;
            Task.Run(() =>
            {
                try
                {
                    if (CancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                    while (true)
                    {
                        try
                        {
                            PurchaseIfNeed();
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Warn(ex);
                        }
                        for (var i = 0; i < 30; i++)
                        {
                            Thread.Sleep(1 * 1000);
                            if (CancelToken.IsCancellationRequested)
                            {
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Error(ex);
                    throw;
                }
                finally
                {
                    LoggerWrapper.Info("End AutoPurcaserMainTask");
                    CancellationTokenSource.Dispose();
                    CancellationTokenSource = null;
                }
            }, CancelToken);
        }

        public void Stop()
        {
            if (!Running)
            {
                return;
            }
            CancellationTokenSource.Cancel();
        }

        private void PurchaseIfNeed()
        {
            try
            {
                var currentStatus = TargetConfigRepository.ReadAll();
                if (currentStatus == null)
                {
                    return;
                }
                foreach (var condition in currentStatus.TargetConditionList)
                {
                    PurchaseSingleRaceIfNeed(condition, currentStatus.PurchasePrice);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Warn(ex);
            }

            void PurchaseSingleRaceIfNeed(TargetCondition condition, int price)
            {
                try
                {
                    if (AlreadyPurchasedRaceHashSet.Contains(condition.RaceData))
                    {
                        return;
                    }
                    if (condition.StartTime > DateTime.Now.AddMinutes(3))
                    {
                        //3分前より近くなったら購入する
                        return;
                    }
#if !DEBUG
                    if (condition.StartTime < DateTime.Now)
                    {
                        return;
                    }
#endif
                    using var scraper = new Scraper();
                    TargetManager.UpdateRealtimeOdds(scraper, condition);

                    if (condition.PurchaseOdds < 1 ||
                        condition.CurrentWinOdds.LowOdds < condition.PurchaseOdds)
                    {
                        AlreadyPurchasedRaceHashSet.Add(condition.RaceData);
                        return;
                    }
                    var betDatum = new BetDatum(
                        condition.RaceData,
                        condition.MidnightWinOdds.HorseData.Select(_ => _.Number).ToList(),
                        price,
                        condition.CurrentWinOdds.LowOdds,
                        condition.CurrentWinOdds.LowOdds,
                        TicketType.Win);
                    var betData = new List<BetDatum>
                    {
                        betDatum
                    };
                    if (betData != null && betData.Any())
                    {
                        LoggerWrapper.Info($"Bet target(s) exist");
                        var loginConfig = new LoginConfigRepository().ReadAll();
                        using var autoPurchaser = new AutoPurchaser(loginConfig);
                        if (autoPurchaser.Purchase(betData))
                        {
                            AlreadyPurchasedRaceHashSet.Add(condition.RaceData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Warn(ex);
                    return;
                }
            }
        }
    }
}