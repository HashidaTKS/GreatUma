using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GreatUma.Utils;
using GreatUma.Domain;
using GreatUma.Infrastructures;

namespace GreatUma.Models
{
    [DataContract]
    [Serializable]
    public class ActualRaceAndOddsData : RaceAndOddsData
    {
        public ActualRaceAndOddsData(): base()
        {
        }

        public ActualRaceAndOddsData(RaceData raceData) : base(raceData)
        {
        }

        /// <summary>
        /// 終了したレースのオッズを設定する。
        /// </summary>
        /// <param name="scraper"></param>
        // Modelに処理があるのであまりよくない。
        public void SetData(Scraper scraper)
        {
            WinOdds = scraper.GetOdds(BaseRaceData, TicketType.Win).Distinct().OrderBy(_ => _.LowOdds).ToList();
            //今は使わないので敢えて取得しない
            //QuinellaOdds = scraper.GetOdds(BaseRaceData, TicketType.Quinella).Distinct().OrderBy(_ => _.LowOdds).ToList();
            //WideOdds = scraper.GetOdds(BaseRaceData, TicketType.Wide).Distinct().OrderBy(_ => _.LowOdds).ToList();
            //ExactaOdds = scraper.GetOdds(BaseRaceData, TicketType.Exacta).Distinct().OrderBy(_ => _.Odds).ToList();
            //TrifectaOdds = scraper.GetOdds(BaseRaceData, TicketType.Trifecta).Distinct().OrderBy(_ => _.Odds).ToList();
            //TrioOdds = scraper.GetOdds(BaseRaceData, TicketType.Trio).Distinct().OrderBy(_ => _.Odds).ToList();
            LastUpadateUtcTime = DateTime.UtcNow;
        }

        /// <summary>
        /// リアルタイムオッズを設定する。
        /// </summary>
        /// <param name="scraper"></param>
        // Modelに処理があるのであまりよくない。
        public void SetRealTimeData(Scraper scraper)
        {
            WinOdds = scraper.GetRealTimeOdds(BaseRaceData, TicketType.Win).Distinct().OrderBy(_ => _.LowOdds).ToList();
            //今は使わないので敢えて取得しない
            //QuinellaOdds = scraper.GetRealTimeOdds(BaseRaceData, TicketType.Quinella).Distinct().OrderBy(_ => _.LowOdds).ToList();
            //WideOdds = scraper.GetRealTimeOdds(BaseRaceData, TicketType.Wide).Distinct().OrderBy(_ => _.LowOdds).ToList();
            //ExactaOdds = scraper.GetRealTimeOdds(BaseRaceData, TicketType.Exacta).Distinct().OrderBy(_ => _.Odds).ToList();
            //TrifectaOdds = scraper.GetRealTimeOdds(BaseRaceData, TicketType.Trifecta).Distinct().OrderBy(_ => _.Odds).ToList();
            //TrioOdds = scraper.GetRealTimeOdds(BaseRaceData, TicketType.Trio).Distinct().OrderBy(_ => _.Odds).ToList();
            LastUpadateUtcTime = DateTime.UtcNow;
        }

        public ActualRaceDataRepository GetRepository()
        {
            return GetRepository(BaseRaceData);
        }

        public static ActualRaceDataRepository GetRepository(RaceData raceData)
        {
            var path = Utility.GetRaceAndOddsDataFilePath(raceData, OddsType.Actual);
            return new ActualRaceDataRepository(path);
        }
    }
}
