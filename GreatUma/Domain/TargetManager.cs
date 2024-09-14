using GreatUma.Utils;
using System.Collections.Generic;
using System.Linq;
using GreatUma.Models;
using GreatUma.Model;
using System.Security.Policy;

namespace GreatUma.Domain
{
    public class TargetManager
    {
        private static double TargetPlaceOdds = 1.5;

        private Scraper Scraper { get; set; }
        internal DateTime TargetDate { get; set; }
        internal List<HorseAndOddsCondition> TargetList { get; private set; } = new List<HorseAndOddsCondition>();

        public bool IsInitialized { get; set; }

        public TargetManager(Scraper scraper, DateTime targetDate)
        {
            this.Scraper = scraper;
            this.TargetDate = targetDate;
        }

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            this.IsInitialized = true;
            SetTargets(TargetDate);
        }

        public void SetTargets(DateTime currentTime)
        {
            var selector = new TargetSelector(this.Scraper, this.TargetDate);
            this.TargetList = selector.GetTargets(currentTime)?.ToList() ?? new List<HorseAndOddsCondition>();
        }

        public void Update(DateTime currentTime)
        {
            this.TargetList.RemoveAll(_ => _.RaceData.StartTime > currentTime);
            if (currentTime.Date > TargetDate)
            {
                SetTargets(TargetDate);
            }
            this.UpdateRealtimeOdds();
        }

        public void UpdateRealtimeOdds()
        {
            var scraper = this.Scraper;
            foreach (var horseAndOddsCondition in this.TargetList)
            {
                var raceData = horseAndOddsCondition.RaceData;
                var placeOddsList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Place);
                var winOddsList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Win);
                if (placeOddsList == null || winOddsList == null)
                {
                    continue;
                }
                if (!int.TryParse(horseAndOddsCondition.HorseNum, out var horseNum))
                {
                    continue;
                }
                // 複勝のオッズなので、馬は一頭。（馬連なら二頭、三連複なら三頭になる。）
                var currentPlaceOdds = placeOddsList
                    .Where(_ => _.HorseData.Count == 1)?
                    .FirstOrDefault(_ => _.HorseData[0].Number == horseNum);
                var currentWinOdds = placeOddsList
                    .Where(_ => _.HorseData.Count == 1)?
                    .FirstOrDefault(_ => _.HorseData[0].Number == horseNum);

                if (currentPlaceOdds == null || currentWinOdds == null)
                {
                    continue;
                }
                horseAndOddsCondition.CurrentOdds = $"単勝:{currentWinOdds.LowOdds} 複勝:{currentPlaceOdds.LowOdds}-{currentPlaceOdds.HighOdds}";

            }
        }
    }
}
