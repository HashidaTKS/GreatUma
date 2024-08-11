using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreatUma.Utils;
using GreatUma.Infrastructures;
using GreatUma.Models;

namespace GreatUma.Domain
{
    public class RaceResultManager
    {
        public static void UpdateResultDataIfNeed(DateTime from, DateTime to)
        {
            using (var scraper = new Scraper())
            {
                UpdateResultDataIfNeed(scraper, from, to);
            }
        }

        public static void UpdateResultDataIfNeed(Scraper scraper, DateTime from, DateTime to)
        {
            var raceDataList = new List<RaceData>();
            for (var date = from; date <= to; date = date.AddDays(1))
            {
                raceDataList.AddRange(RaceDataManager.GetAndStoreRaceDataOfDay(date, scraper));
            }

            foreach (var raceData in raceDataList)
            {
                if (raceData == null)
                {
                    continue;
                }
                var raceResult = new RaceResult(raceData);
                var repo = raceResult.GetRepository();
                var currenRaceResult = repo.ReadAll();
                if (currenRaceResult == null)
                {
                    raceResult = scraper.GetRaceResult(raceData);
                    if (raceResult == null)
                    {
                        continue;
                    }
                    repo.Store(raceResult);
                }
            }
        }

        public static void UpdateResultDataIfNeed(Scraper scraper, RaceData raceData)
        {

            if (raceData == null)
            {
                return;
            }
            var raceResult = new RaceResult(raceData);
            var repo = raceResult.GetRepository();
            var currenRaceResult = repo.ReadAll();
            if (currenRaceResult == null)
            {
                raceResult = scraper.GetRaceResult(raceData);
                if (raceResult == null)
                {
                    return;
                }
                repo.Store(raceResult);
            }
        }
    }
}
