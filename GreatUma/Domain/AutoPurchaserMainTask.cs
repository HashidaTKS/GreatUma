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
        private BetConfig BetConfig { get; set; }
        private LoginConfig LoginConfig { get; set; }
        private TargetStatusRepository TargetStatusRepository { get; set; }
        private HashSet<RaceData> AlreadyPurchasedRaceHashSet { get; set; }

        public void Run()
        {
            if (Running)
            {
                return;
            }
            LoggerWrapper.Info("Start AutoPurcaserMainTask");
            CancellationTokenSource = new CancellationTokenSource();
            CancelToken = CancellationTokenSource.Token;
            BetConfig = new BetConfigRepository().ReadAll();
            LoginConfig = new LoginConfigRepository().ReadAll();
            Task.Run(() =>
            {
                try
                {
                    if (CancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                    using (var scraper = new Scraper())
                    using (var autoPurchaser = new AutoPurchaser(LoginConfig))
                    {
                        while (true)
                        {
                            try
                            {
                                PurchaseIfNeed(scraper, autoPurchaser);
                                UpdateResult(scraper);
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

        //        /// <summary>
        //        ///必要に応じて結果データを更新しながら、過去のベットの結果を確認する。
        //        ///ただし最大でも一カ月前までしか遡らない。
        //        ///また、レースデータが存在している部分のみを対象とする。
        //        /// </summary>
        //        /// <param name="scraper"></param>
        //        private void UpdateResult(Scraper scraper)
        //        {
        //            var betResultStatus = BetResultStatusRepository.ReadAll(true);
        //            var monthBefore = DateTime.Now.AddMonths(-1);
        //            var statusCheckTargetTime = DateTime.Now.AddHours(-1);
        //            var statusCheckStart = betResultStatus.CheckedTime > monthBefore ? betResultStatus.CheckedTime : monthBefore;


        //            for (var date = statusCheckStart; date < statusCheckTargetTime; date = date.AddDays(1))
        //            {
        //                try
        //                {
        //                    if (CancelToken.IsCancellationRequested)
        //                    {
        //                        return;
        //                    }
        //                    foreach (var targetRace in RaceDataManager.GetRaceDataOfDay(date.Date))
        //                    {
        //                        if (targetRace == null)
        //                        {
        //                            LoggerWrapper.Debug("Target race does not exist");
        //                            continue;
        //                        }
        //                        if (targetRace.StartTime > statusCheckTargetTime)
        //                        {
        //                            //本日のまだ確定していないデータの可能性があるので、スキップ
        //                            LoggerWrapper.Debug("Do not elapse enough time");
        //                            continue;
        //                        }
        //                        if (targetRace.StartTime < statusCheckStart)
        //                        {
        //                            //確認済みなのでスキップ
        //                            LoggerWrapper.Debug("Already checked");
        //                            continue;
        //                        }


        //                        try
        //                        {
        //                            RaceResultManager.UpdateResultDataIfNeed(scraper, targetRace);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            LoggerWrapper.Warn(ex);
        //                            continue;
        //                        }

        //                        var betInformation = BetInformation.GetRepository(targetRace).ReadAll();
        //                        if (betInformation == null)
        //                        {
        //                            continue;
        //                        }

        //                        var raceResult = RaceResult.GetRepository(targetRace).ReadAll();
        //                        foreach (var betDatum in betInformation.BetData)
        //                        {
        //                            var resultOfBet = new ResultOfBet(betDatum, raceResult);
        //                            var targetStatus = betResultStatus.GetTicketTypeStatus(betDatum.TicketType);
        //                            if (resultOfBet.IsHit)
        //                            {
        //                                targetStatus.CountOfContinuationLose = 0;
        //                            }
        //                            else
        //                            {
        //                                targetStatus.CountOfContinuationLose += 1;
        //                            }

        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    LoggerWrapper.Warn(ex);
        //                    continue;
        //                }
        //            }

        //            betResultStatus.CheckedTime = statusCheckTargetTime;
        //            BetResultStatusRepository.Store(betResultStatus);
        //        }


        private void PurchaseIfNeed(Scraper scraper, AutoPurchaser autoPurchaser)
        {
            try
            {
                var currentStatus = TargetStatusRepository.ReadAll();
                foreach (var condition in currentStatus.HorseAndOddsConditionList)
                {
                    PurchaseSingleRaceIfNeed(condition);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Warn(ex);
            }

            void PurchaseSingleRaceIfNeed(HorseAndOddsCondition targetRace)
            {
                try
                {
                    if (AlreadyPurchasedRaceHashSet.Contains(targetRace.RaceData))
                    {
                        return;
                    }
                    TargetManager.UpdateRealtimeOdds(scraper, targetRace);
                    if (targetRace.CurrentWinOdds.LowOdds < 10)
                    {
                        return;
                    }
                    var betDatum = new BetDatum(
                        targetRace.RaceData, 
                        targetRace.MidnightWinOdds.HorseData.Select(_ => _.Number).ToList(), 
                        100, 
                        targetRace.CurrentWinOdds.LowOdds, 
                        targetRace.CurrentWinOdds.LowOdds, 
                        TicketType.Win);
                    var betData = new List<BetDatum>
                    {
                        betDatum
                    };
                    if (betData != null && betData.Any())
                    {
                        LoggerWrapper.Info($"Bet target(s) exist");
                        if (autoPurchaser.Purchase(betData))
                        {
                            //var betInformation = new BetInformation(, betData);
                            //var betInfoRepo = betInformation.GetRepository();
                            //betInfoRepo.Store(betInformation);
                            AlreadyPurchasedRaceHashSet.Add(targetRace.RaceData);
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