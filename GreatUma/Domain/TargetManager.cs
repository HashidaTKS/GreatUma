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

        public TargetManager(DateTime targetDate, TargetConfigRepository targetConfigRepository)
        {
            this.TargetDate = targetDate;
            this.TargetConfigRepository = targetConfigRepository;
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
            var targetStatus = TargetConfigRepository.ReadAll(true);
            var selector = new TargetSelector(scraper, this.TargetDate, targetStatus.TargetPlaceOdds);
            var targetList = selector.GetTargets(currentTime)?.OrderBy(_ => _.StartTime).ToList() ?? new List<TargetCondition>();
            if (targetStatus.TargetConditionList != null)
            {
                foreach (var target in targetList)
                {
                    var currentCondition = targetStatus.TargetConditionList.FirstOrDefault(_ => _.RaceData.Equals(target.RaceData));
                    if (currentCondition == null)
                    {
                        continue;
                    }
                    target.PurchaseOdds = currentCondition.PurchaseOdds;
                }
            }
            targetStatus.TargetConditionList = targetList;
            TargetConfigRepository.Store(targetStatus);
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
            var targetStatus = TargetConfigRepository.ReadAll(true);
            try
            {
                if (targetStatus.TargetConditionList == null)
                {
                    return;
                }
                foreach (var targetCondition in targetStatus.TargetConditionList)
                {
                    UpdateRealtimeOdds(scraper, targetCondition);
                }
            }
            finally
            {
                TargetConfigRepository.Store(targetStatus);
            }
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
    }
}
