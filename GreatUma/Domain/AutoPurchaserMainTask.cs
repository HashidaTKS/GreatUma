using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreatUma.Utils;
using GreatUma.Infrastructures;
using GreatUma.Models;

namespace GreatUma.Domain
{
    public class AutoPurchaserMainTask
    {
        public bool Running => CancellationTokenSource != null;

        private CancellationTokenSource CancellationTokenSource { get; set; }
        private CancellationToken CancelToken { get; set; }
        private BetConfig BetConfig { get; set; }
        private LoginConfig LoginConfig { get; set; }
        private BetResultStatusRepository BetResultStatusRepository { get; set; }
        private List<ActualRaceAndOddsData> NotSavedBetRaceList { get; set; }

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
            BetResultStatusRepository = new BetResultStatusRepository();
            //Store時にエラーが起きたなどの場合に重複ベットしないためのメモ
            NotSavedBetRaceList = new List<ActualRaceAndOddsData>();

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

        /// <summary>
        ///必要に応じて結果データを更新しながら、過去のベットの結果を確認する。
        ///ただし最大でも一カ月前までしか遡らない。
        ///また、レースデータが存在している部分のみを対象とする。
        /// </summary>
        /// <param name="scraper"></param>
        private void UpdateResult(Scraper scraper)
        {
            var betResultStatus = BetResultStatusRepository.ReadAll(true);
            var monthBefore = DateTime.Now.AddMonths(-1);
            var statusCheckTargetTime = DateTime.Now.AddHours(-1);
            var statusCheckStart = betResultStatus.CheckedTime > monthBefore ? betResultStatus.CheckedTime : monthBefore;


            for (var date = statusCheckStart; date < statusCheckTargetTime; date = date.AddDays(1))
            {
                try
                {
                    if (CancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                    foreach (var targetRace in RaceDataManager.GetRaceDataOfDay(date.Date))
                    {
                        if (targetRace == null)
                        {
                            LoggerWrapper.Debug("Target race does not exist");
                            continue;
                        }
                        if (targetRace.StartTime > statusCheckTargetTime)
                        {
                            //本日のまだ確定していないデータの可能性があるので、スキップ
                            LoggerWrapper.Debug("Do not elapse enough time");
                            continue;
                        }
                        if (targetRace.StartTime < statusCheckStart)
                        {
                            //確認済みなのでスキップ
                            LoggerWrapper.Debug("Already checked");
                            continue;
                        }


                        try
                        {
                            RaceResultManager.UpdateResultDataIfNeed(scraper, targetRace);
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Warn(ex);
                            continue;
                        }

                        var betInformation = BetInformation.GetRepository(targetRace).ReadAll();
                        if (betInformation == null)
                        {
                            continue;
                        }

                        var raceResult = RaceResult.GetRepository(targetRace).ReadAll();
                        foreach (var betDatum in betInformation.BetData)
                        {
                            var resultOfBet = new ResultOfBet(betDatum, raceResult);
                            var targetStatus = betResultStatus.GetTicketTypeStatus(betDatum.TicketType);
                            if (resultOfBet.IsHit)
                            {
                                targetStatus.CountOfContinuationLose = 0;
                            }
                            else
                            {
                                targetStatus.CountOfContinuationLose += 1;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Warn(ex);
                    continue;
                }
            }

            betResultStatus.CheckedTime = statusCheckTargetTime;
            BetResultStatusRepository.Store(betResultStatus);
        }


        private void PurchaseIfNeed(Scraper scraper, AutoPurchaser autoPurchaser)
        {
            try
            {
                var targetRaces = GetTargetRaceList();
                foreach (var targetRace in targetRaces)
                {
                    PurchaseSingleRaceIfNeed(targetRace);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Warn(ex);
            }

            IEnumerable<RaceData> GetTargetRaceList()
            {
                var today = DateTime.Today;
                var raceData = RaceDataManager.GetAndStoreRaceDataOfDay(today, scraper).ToList();
                if (BetConfig.ContainCentral())
                {
                    foreach(var data in raceData
                        .Where(_ => _.HoldingDatum.Region.RagionType == RegionType.Central)
                        .Where(_ => _.StartTime >= DateTime.Now && _.StartTime <= DateTime.Now.AddMinutes(5)))
                    {
                        yield return data;
                    }
                }
                if (BetConfig.ContainRegional())
                {
                    //ばんえいは除く。
                    foreach (var data in raceData
                        .Where(_ => _.HoldingDatum.Region.RagionType == RegionType.Regional && _.HoldingDatum.Region.RegionId != "65")
                        .Where(_ => _.StartTime >= DateTime.Now && _.StartTime <= DateTime.Now.AddMinutes(5))){
                        yield return data;
                    }
                }
            }

            void PurchaseSingleRaceIfNeed(RaceData targetRace)
            {
                try
                {
                    foreach (var notSavedBetRace in NotSavedBetRaceList)
                    {
                        var repository = notSavedBetRace.GetRepository();
                        repository.Store(notSavedBetRace);
                    }
                    NotSavedBetRaceList.Clear();

                    var actualRaceAndOddsData = new ActualRaceAndOddsData(targetRace);
                    var actualRaceAndOddsDataRepository = actualRaceAndOddsData.GetRepository();
                    var savedData = actualRaceAndOddsDataRepository.ReadAll();
                    if (savedData == null)
                    {
                        actualRaceAndOddsData.SetRealTimeData(scraper);
                    }
                    else
                    {
                        //保存されているということは購入済みなのでスキップする
                        return;
                    }

                    var notSavedBets = NotSavedBetRaceList.Where(_ => _.BaseRaceData.Equals(actualRaceAndOddsData.BaseRaceData));
                    foreach(var notSavedBet in notSavedBets)
                    {
                        actualRaceAndOddsDataRepository.Store(notSavedBet);
                        NotSavedBetRaceList.Remove(notSavedBet);
                        return;
                    }

                    var betResultStatus = BetResultStatusRepository.ReadAll(true);
                    var betData = TicketSelector.SelectToBet(actualRaceAndOddsData, BetConfig, betResultStatus).ToList();

                    if (betData != null && betData.Any())
                    {
                        LoggerWrapper.Info($"Bet target(s) exist");
                        if (autoPurchaser.Purchase(betData))
                        {
                            var betInformation = new BetInformation(targetRace, betData);
                            var betInfoRepo = betInformation.GetRepository();
                            betInfoRepo.Store(betInformation);
                        }
                    }
                    NotSavedBetRaceList.Add(actualRaceAndOddsData);
                    actualRaceAndOddsDataRepository.Store(actualRaceAndOddsData);
                    NotSavedBetRaceList.Remove(actualRaceAndOddsData);
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