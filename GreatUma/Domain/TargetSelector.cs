using GreatUma.Utils;
using System.Collections.Generic;
using System.Linq;
using GreatUma.Models;
using GreatUma.Model;
using System.Security.Policy;
using NLog.Common;

namespace GreatUma.Domain
{
    public class TargetSelector
    {
        private double TargetPlaceOdds { get; set; } = 1.8;
        private Scraper Scraper { get; set; }
        private DateTime TargetDate { get; set; }
        
        public TargetSelector(Scraper scraper, DateTime targetDate, double targetPlaceOdds = 1.1)
        {
            this.Scraper = scraper;
            this.TargetDate = targetDate;
            this.TargetPlaceOdds = targetPlaceOdds; 
        }

        public IEnumerable<HorseAndOddsCondition> GetTargets(DateTime currentTime)
        {
            var scraper = this.Scraper;
            var targetDate = this.TargetDate;
            var holdingInformation = scraper.GetHoldingInformation(targetDate, RegionType.Central);
            if (holdingInformation == null)
            {
                yield break;
            }
            foreach (var holdingDatum in holdingInformation.HoldingData)
            {
                var totalCount = holdingDatum.TotalRaceCount;
                for (var i = 0; i < totalCount; i++)
                {
                    var startTime = holdingDatum.StartTimeList[i];
                    if (startTime < currentTime)
                    {
                        continue;
                    }
                    var raceData = new RaceData(holdingDatum, i + 1);
                    var horseAndOddsCondition = GetTarget(raceData);
                    if (horseAndOddsCondition != null)
                    {
                        yield return horseAndOddsCondition;
                    }
                }
            }
        }

        public HorseAndOddsCondition GetTarget(RaceData raceData)
        {
            try
            {
                var scraper = this.Scraper;
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
                if (mostPopularPlace.HighOdds > TargetPlaceOdds)
                {
                    return null;
                }
                return new HorseAndOddsCondition()
                {
                    PurchaseCondition = -1,
                    RaceData = raceData,
                    MidnightWinOdds = mostPopularWin,
                    MidnightPlaceOdds = mostPopularPlace,
                    CurrentWinOdds = mostPopularWin,
                    CurrentPlaceOdds = mostPopularPlace,
                };
            }
            catch(Exception ex)
            {
                LoggerWrapper.Error(ex);
                return null;
            }
        }
    }
}
