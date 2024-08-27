using GreatUma.Utils;
using System.Collections.Generic;
using System.Linq;
using GreatUma.Models;
using GreatUma.Model;

namespace GreatUma.Domain
{
    public class TargetSelector()
    {
        private static double TargetPlaceOdds = 1.1;

        public IEnumerable<HorseAndOddsCondition> GetTargets(Scraper scraper, RaceData raceData)
        {
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
                    RaceClass = "",
                    Course = raceData.CourseType.ToString(),
                    HorseNum = horseNumberForOdds.ToString(),
                    Jocky = horseDatum.Jockey,
                    MidnightOdds = $"{mostPopularWin.LowOdds} {mostPopularPlace.LowOdds}-{mostPopularPlace.HighOdds}",
                    CurrentOdds = "",
                    PurchaseCondition = -1,
                };
            }
        }
    }
}
