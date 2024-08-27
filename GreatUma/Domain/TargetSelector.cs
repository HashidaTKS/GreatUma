using GreatUma.Utils;
using System.Collections.Generic;
using System.Linq;
using GreatUma.Models;
using GreatUma.Model;

namespace GreatUma.Domain
{
    internal class TargetSelector()
    {
        private static double TargetPlaceOdds = 1.1; 

        internal IEnumerable<HorseAndOddsCondition> GetTargets(RaceData raceData, List<OddsDatum> oddsData, List<HorseDatum> horseData)
        {
            foreach(var oddsDatum in oddsData.Where(_ => _.HighOdds < TargetPlaceOdds))
            {
                if (oddsDatum.HorseData.Count != 1)
                {
                    // 複勝のオッズなので、馬は一頭。（馬連なら二頭、三連複なら三頭になる。）
                    continue;
                }
                var horseNumberForOdds = oddsDatum.HorseData.FirstOrDefault().Number;
                var horseDatum = horseData.FirstOrDefault(_ => _.Number == horseNumberForOdds);
                if(horseDatum != null)
                {
                    yield return new HorseAndOddsCondition()
                    {
                        StartTime = raceData.StartTime, 
                    };
                }
            }

        }
    }
}
