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
        private static double TargetPlaceOdds = 1.1;

        private Scraper Scraper { get; set; }
        internal DateTime TargetDate { get; set; }
        private Dictionary<RaceData, List<HorseAndOddsCondition>> TargetDictionary { get; set; } = new Dictionary<RaceData, List<HorseAndOddsCondition>>();

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
            SetTargets(TargetDate);
            this.IsInitialized = true;
        }

        /// <summary>
        /// 現在の対象一覧を返却する。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HorseAndOddsCondition> GetConditions()
        {
            // TargetDictionaryを平坦化し、HorseAndOddsConditionの情報のみを返却する。
            foreach (var horseAndOddsConditionList in this.TargetDictionary.Values)
            {
                foreach(var horseAndOddsCondition in horseAndOddsConditionList)
                {
                    yield return horseAndOddsCondition;
                }
            }
        }

        public void SetTargets(DateTime currentTime)
        {
            var selector = new TargetSelector(this.Scraper, this.TargetDate);
            var targets = selector.GetTargets(currentTime)?.ToList() ?? new List<(RaceData, List<HorseAndOddsCondition>)>();
            var targetDictionary = targets.ToDictionary(_ => _.Item1, _ => _.Item2);
            this.TargetDictionary = targetDictionary;
        }

        public void Update(DateTime currentTime)
        {
            this.TargetDictionary = this.TargetDictionary.
                Where(_ => _.Key.StartTime > currentTime)?.
                ToDictionary() ?? new Dictionary<RaceData, List<HorseAndOddsCondition>>();
            this.UpdateRealtimeOdds();
        }

        public void UpdateRealtimeOdds()
        {
            var scraper = this.Scraper;
            foreach (var raceData in this.TargetDictionary.Keys)
            {
                var horseAndOdds = this.TargetDictionary[raceData];
                var placeOddsList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Place);
                var winOddsList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Win);
                if (placeOddsList == null || winOddsList == null)
                {
                    continue;
                }
                foreach (var horseAndOddsDatum in horseAndOdds)
                {
                    if (!int.TryParse(horseAndOddsDatum.HorseNum, out var horseNum))
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
                    horseAndOddsDatum.CurrentOdds = $"{currentWinOdds.LowOdds} {currentPlaceOdds.LowOdds}-{currentPlaceOdds.HighOdds}";
                }
            }
        }
    }
}
