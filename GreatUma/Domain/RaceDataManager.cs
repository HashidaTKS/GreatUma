using GreatUma.Utils;
using GreatUma.Infrastructures;
using GreatUma.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GreatUma.Domain
{
    public class RaceDataManager
    {
        public static IEnumerable<RaceData> GetRaceDataOfDay(DateTime date)
        {
            var holdingInformationRepository = new HoldingInformationRepository();
            var currentHoldingInformation = holdingInformationRepository.ReadAll();

            var holdingData = new List<HoldingDatum>();
            try
            {
                holdingData = currentHoldingInformation.HoldingData.Where(_ => _.HeldDate.Date == date.Date).ToList();
            }
            catch (Exception ex)
            {
                LoggerWrapper.Warn(ex);
                yield break;
            }

            foreach (var holdingDatum in holdingData)
            {
                if (holdingDatum == null || !holdingDatum.HasFullData)
                {
                    continue;
                }
                for (var raceNumber = 1; raceNumber <= holdingDatum.TotalRaceCount; raceNumber++)
                {
                    yield return new RaceData(holdingDatum, raceNumber);
                }
            }
        }

        public static IEnumerable<RaceData> GetAndStoreRaceDataOfDay(DateTime date, Scraper scraper)
        {
            var holdingInformationRepository = new HoldingInformationRepository();
            var currentHoldingInformation = holdingInformationRepository.ReadAll() ?? new HoldingInformation(new List<HoldingDatum>());


            if (!currentHoldingInformation.HoldingData.Any(_ => _.HeldDate.Date == date.Date))
            {
                //別々に取得しなきゃいけないのおかしい気もする
                var centralHoldingInformation = scraper.GetHoldingInformation(date, RegionType.Central);
                var regionalHoldingInformation = scraper.GetHoldingInformation(date, RegionType.Regional);

                currentHoldingInformation = currentHoldingInformation.MergeStatus(centralHoldingInformation).MergeStatus(regionalHoldingInformation);
                new HoldingInformationRepository().Store(currentHoldingInformation);
            }

            var holdingData = currentHoldingInformation.HoldingData.Where(_ => _.HeldDate.Date == date.Date).ToList();

            foreach (var holdingDatum in holdingData)
            {
                if (holdingDatum == null || !holdingDatum.HasFullData)
                {
                    continue;
                }
                for (var raceNumber = 1; raceNumber <= holdingDatum.TotalRaceCount; raceNumber++)
                {
                    yield return new RaceData(holdingDatum, raceNumber);
                }
            }
        }
    }
}
