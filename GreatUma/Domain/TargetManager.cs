using GreatUma.Utils;
using System.Collections.Generic;
using System.Linq;
using GreatUma.Models;
using GreatUma.Model;
using System.Security.Policy;
using GreatUma.Infrastructure;

namespace GreatUma.Domain
{
    public class TargetManager
    {
        internal DateTime TargetDate { get; set; }
        public bool IsInitialized { get; set; }
        private TargetConfigRepository TargetConfigRepository { get; set; }
        private WholeTargetConditionsRepository WholeTargetConditionsRepository { get; set; }

        public TargetManager(DateTime targetDate, TargetConfigRepository targetConfigRepository)
        {
            this.TargetDate = targetDate;
            this.TargetConfigRepository = targetConfigRepository;
            WholeTargetConditionsRepository = new WholeTargetConditionsRepository();
        }

        /// <summary>
        /// 設定の取得、更新を行う。
        /// * 最新の設定の取得、更新を行う
        /// * 最新のオッズが条件を満たしていたら設定に追加する
        /// </summary>
        /// <param name="currentTime"></param>
        public void SetTargets(DateTime currentTime)
        {
            if (currentTime.Date > TargetDate)
            {
                TargetDate = currentTime.Date;
            }
            using var scraper = new Scraper();
            StoreCurrentAllTargetsIfNeed(scraper);
            this.UpdateAllRealtimeOdds();
            var targetStatus = TargetConfigRepository.ReadAll(true);
            var targetList = SearchMatchedTargetConditions(currentTime)?.OrderBy(_ => _.StartTime).ToList() ?? new List<TargetCondition>();
            if (targetStatus.TargetConditionList != null)
            {
                foreach (var target in targetList)
                {
                    var currentCondition = targetStatus.TargetConditionList.FirstOrDefault(_ => _.RaceData.Equals(target.RaceData));
                    if (currentCondition == null)
                    {
                        target.MatchedDateTime = DateTime.Now;
                        target.MatchedWinOdds.HighOdds = target.CurrentWinOdds.HighOdds;
                        target.MatchedWinOdds.LowOdds = target.CurrentWinOdds.LowOdds;
                        target.MatchedPlaceOdds.HighOdds = target.CurrentPlaceOdds.HighOdds;
                        target.MatchedPlaceOdds.LowOdds = target.CurrentPlaceOdds.LowOdds;
                    }
                    target.PurchaseOdds = currentCondition.PurchaseOdds;
                }
            }
            targetStatus.TargetConditionList = targetList;
            TargetConfigRepository.Store(targetStatus);
        }

        public void UpdateAllRealtimeOdds()
        {
            using var scraper = new Scraper();
            var currentWholeConditions = WholeTargetConditionsRepository.ReadAll(true);
            var currentCondition = currentWholeConditions.TargetConditionsOfDay?.FirstOrDefault(_ => _.TargetDate == TargetDate);
            if (currentCondition == null)
            {
                return;
            }
            if (currentCondition.TargetConditionList == null)
            {
                return;
            }
            foreach (var targetCondition in currentCondition.TargetConditionList)
            {
                UpdateRealtimeOdds(scraper, targetCondition);
            }
            WholeTargetConditionsRepository.Store(currentWholeConditions);
        }

        public static void UpdateRealtimeOdds(Scraper scraper, TargetCondition targetCondition)
        {
            var raceData = targetCondition.RaceData;
            //ここで取得したOddsデータには、あまり詳細なデータが入っていないことに注意。
            var placeOddsList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Place);
            var winOddsList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Win);
            if (placeOddsList == null || winOddsList == null)
            {
                return;
            }
            if (!int.TryParse(targetCondition.HorseNum, out var horseNum))
            {
                return;
            }
            // 複勝のオッズなので、馬は一頭。（馬連なら二頭、三連複なら三頭になる。）
            var currentPlaceOdds = placeOddsList
                .Where(_ => _.HorseData.Count == 1)?
                .FirstOrDefault(_ => _.HorseData[0].Number == horseNum);
            var currentWinOdds = winOddsList
                .Where(_ => _.HorseData.Count == 1)?
                .FirstOrDefault(_ => _.HorseData[0].Number == horseNum);

            if (currentPlaceOdds == null || currentWinOdds == null)
            {
                return;
            }
            targetCondition.CurrentWinOdds = currentWinOdds;
            targetCondition.CurrentPlaceOdds = currentPlaceOdds;
        }

        public void StoreCurrentAllTargetsIfNeed(Scraper scraper)
        {
            var currentWholeConditions = WholeTargetConditionsRepository.ReadAll(true);
            var currentCondition = currentWholeConditions?.TargetConditionsOfDay?.FirstOrDefault(_ => _.TargetDate == TargetDate);
            if (currentWholeConditions != null)
            {
                return;
            }
            var targetDate = this.TargetDate;
            var holdingInformation = scraper.GetHoldingInformation(targetDate, RegionType.Central);
            if (holdingInformation == null)
            {
                return;
            }
            currentWholeConditions ??= new WholeTargetConditions();
            currentWholeConditions.TargetConditionsOfDay ??= new List<TargetConditionsOfDay>();
            var targetConditionOfDay = new TargetConditionsOfDay()
            {
                TargetDate = TargetDate
            };
            foreach (var holdingDatum in holdingInformation.HoldingData)
            {
                var totalCount = holdingDatum.TotalRaceCount;
                for (var i = 0; i < totalCount; i++)
                {
                    var startTime = holdingDatum.StartTimeList[i];
                    var raceData = new RaceData(holdingDatum, i + 1);
                    var targetCondition = CreateTargetCondition(raceData, scraper);
                    if (targetCondition != null)
                    {
                        targetConditionOfDay.TargetConditionList.Add(targetCondition);
                    }
                }
            }
            currentWholeConditions.TargetConditionsOfDay.Add(targetConditionOfDay);
            WholeTargetConditionsRepository.Store(currentWholeConditions);
        }

        public IEnumerable<TargetCondition> SearchMatchedTargetConditions(DateTime currentTime)
        {
            var currentWholeConditions = WholeTargetConditionsRepository.ReadAll(true);
            var currentCondition = currentWholeConditions.TargetConditionsOfDay.FirstOrDefault(_ => _.TargetDate == TargetDate);
            if (currentCondition == null)
            {
                yield break;
            }
            var targetStatus = TargetConfigRepository.ReadAll(true);
            var targetPlaceOdds = targetStatus == null ? 1.1 :
                targetStatus.TargetPlaceOdds;
            var currentTargets = currentCondition.TargetConditionList
                .Where(_ => _.StartTime > currentTime)
                .Where(_ => _.CurrentPlaceOdds.HighOdds > targetPlaceOdds);
            foreach (var target in currentTargets)
            {
                yield return target;
            }
        }

        public TargetCondition CreateTargetCondition(RaceData raceData, Scraper scraper)
        {
            try
            {
                var placeOddsList = scraper.GetOdds(raceData, Utils.TicketType.Place);
                var winOddsList = scraper.GetOdds(raceData, Utils.TicketType.Win);
                var horseData = scraper.GetHorseInfo(raceData);
                var mostPopularWin = winOddsList.OrderBy(_ => _.LowOdds).FirstOrDefault();
                if (mostPopularWin == null)
                {
                    return null;
                }
                if (mostPopularWin.HorseData.Count != 1)
                {
                    // 複勝のオッズなので、馬は一頭。（馬連なら二頭、三連複なら三頭になる。）
                    return null;
                }
                var mostPopularPlace = placeOddsList
                        .Where(_ => _.HorseData.Count == 1)?
                        .FirstOrDefault(_ => _.HorseData[0].Number == mostPopularWin.HorseData[0].Number);
                if (mostPopularPlace == null)
                {
                    return null;
                }
                return new TargetCondition()
                {
                    PurchaseOdds = -1,
                    RaceData = raceData,
                    MatchedWinOdds = mostPopularWin,
                    MatchedPlaceOdds = mostPopularPlace,
                    CurrentWinOdds = mostPopularWin,
                    CurrentPlaceOdds = mostPopularPlace,
                };
            }
            catch (Exception ex)
            {
                LoggerWrapper.Error(ex);
                return null;
            }
        }
    }
}
