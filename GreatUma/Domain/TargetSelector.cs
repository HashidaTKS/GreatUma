using GreatUma.Utils;
using System.Collections.Generic;
using System.Linq;
using GreatUma.Models;
using GreatUma.Model;
using System.Security.Policy;

namespace GreatUma.Domain
{
    public class TargetSelector
    {
        private static double TargetPlaceOdds = 1.1;

        private Scraper Scraper {  get; set; }
        private DateTime TargetDate { get; set; }

        public TargetSelector(Scraper scraper, DateTime targetDate)
        {
            this.Scraper = scraper;
            this.TargetDate = targetDate;
        }

        public IEnumerable<(RaceData, List<HorseAndOddsCondition>)> GetTargets(DateTime currentTime)
        {
            var scraper = this.Scraper;
            var targetDate = this.TargetDate;
            var holdingInformation = scraper.GetHoldingInformation(targetDate, RegionType.Central);
            if (holdingInformation == null)
            {
                yield break;
            }
            foreach(var holdingDatum in holdingInformation.HoldingData)
            {
                var totalCount = holdingDatum.TotalRaceCount;
                for(var i = 0; i < totalCount; i++)
                {
                    var startTime = holdingDatum.StartTimeList[i];
                    if (startTime < currentTime)
                    {
                        continue;
                    }
                    var raceData = new RaceData(holdingDatum, i + 1);
                    var horseAndOddsConditionList = GetTargets(raceData).ToList();
                    yield return (raceData, horseAndOddsConditionList);
                }
            }
        }

        public IEnumerable<HorseAndOddsCondition> GetTargets(RaceData raceData)
        {
            var scraper = this.Scraper;
            var placeOddsList = scraper.GetOdds(raceData, Utils.TicketType.Place);
            var winOddsList = scraper.GetOdds(raceData, Utils.TicketType.Win);
            var horseData = scraper.GetHorseInfo(raceData);
            var mostPopularWin = winOddsList.OrderBy(_ => _.LowOdds).FirstOrDefault();
            if (mostPopularWin == null)
            {
                yield break;
            }
            if (mostPopularWin.HorseData.Count != 1)
            {
                // 複勝のオッズなので、馬は一頭。（馬連なら二頭、三連複なら三頭になる。）
                yield break;
            }
            var mostPopularPlace = placeOddsList
                    .Where(_ => _.HorseData.Count == 1)?
                    .FirstOrDefault(_ => _.HorseData[0].Number == mostPopularWin.HorseData[0].Number);
            if (mostPopularPlace == null)
            {
                yield break;
            }
            var horseNumberForOdds = mostPopularWin.HorseData[0].Number;
            var horseDatum = horseData.FirstOrDefault(_ => _.Number == horseNumberForOdds);
            if (horseDatum != null)
            {
                yield return new HorseAndOddsCondition()
                {
                    StartTime = raceData.StartTime,
                    Region = raceData.HoldingDatum.Region.RegionName,
                    Title = raceData.Title,
                    Course = raceData.CourseType.ToString(),
                    HorseNum = horseNumberForOdds.ToString(),
                    Jocky = horseDatum.Jockey,
                    MidnightOdds = $"{mostPopularWin.LowOdds} {mostPopularPlace.LowOdds}-{mostPopularPlace.HighOdds}",
                    CurrentOdds = $"{mostPopularWin.LowOdds} {mostPopularPlace.LowOdds}-{mostPopularPlace.HighOdds}",
                    PurchaseCondition = -1,
                };
            }
        }
    }
}
