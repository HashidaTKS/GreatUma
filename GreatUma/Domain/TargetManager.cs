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
        private double TargetPlaceOdds { get; set; }
        internal DateTime TargetDate { get; set; }
        public bool IsInitialized { get; set; }
        private TargetStatusRepository TargetStatusRepository { get; set; }

        public TargetManager(DateTime targetDate, TargetStatusRepository targetStatusRepository, double targetPlaceOdds = 1.1)
        {
            this.TargetDate = targetDate;
            this.TargetPlaceOdds = targetPlaceOdds;
            this.TargetStatusRepository = targetStatusRepository;
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
            using var scraper = new Scraper();
            var selector = new TargetSelector(scraper, this.TargetDate, this.TargetPlaceOdds);
            var targetList = selector.GetTargets(currentTime)?.OrderBy(_ => _.StartTime).ToList() ?? new List<HorseAndOddsCondition>();
            var targetStatus = TargetStatusRepository.ReadAll(true);
            if (targetStatus.HorseAndOddsConditionList != null)
            {
                foreach (var target in targetList)
                {
                    var currentCondition = targetStatus.HorseAndOddsConditionList.FirstOrDefault(_ => _.RaceData.Equals(target.RaceData));
                    if (currentCondition == null)
                    {
                        continue;
                    }
                    target.PurchaseCondition = currentCondition.PurchaseCondition;
                }
            }
            targetStatus.HorseAndOddsConditionList = targetList;
            TargetStatusRepository.Store(targetStatus);
        }

        public void Update(DateTime currentTime)
        {
            if (currentTime.Date > TargetDate)
            {
                TargetDate = currentTime.Date;
                SetTargets(TargetDate);
            }
            this.UpdateAllRealtimeOdds();
        }

        public void UpdateAllRealtimeOdds()
        {
            using var scraper = new Scraper();
            var targetStatus = TargetStatusRepository.ReadAll(true);
            try
            {
                if (targetStatus.HorseAndOddsConditionList == null)
                {
                    return;
                }
                foreach (var horseAndOddsCondition in targetStatus.HorseAndOddsConditionList)
                {
                    UpdateRealtimeOdds(scraper, horseAndOddsCondition);
                }
            }
            finally
            {
                TargetStatusRepository.Store(targetStatus);
            }
        }

        public static void UpdateRealtimeOdds(Scraper scraper, HorseAndOddsCondition horseAndOddsCondition)
        {
            var raceData = horseAndOddsCondition.RaceData;
            //ここで取得したOddsデータには、あまり詳細なデータが入っていないことに注意。
            var placeOddsList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Place);
            var winOddsList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Win);
            if (placeOddsList == null || winOddsList == null)
            {
                return;
            }
            if (!int.TryParse(horseAndOddsCondition.HorseNum, out var horseNum))
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
            horseAndOddsCondition.CurrentWinOdds = currentWinOdds;
            horseAndOddsCondition.CurrentPlaceOdds = currentPlaceOdds;
        }
    }
}
