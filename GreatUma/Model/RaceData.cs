using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using GreatUma.Utils;


namespace GreatUma.Models
{
    //現在はRaceDataがHoldingDatumを持っているが、これは参照関係がおかしい。
    //HoldingDatumはRaceDataの集合体であるべき。
    [DataContract]
    [Serializable]
    public class RaceData
    {
        [DataMember]
        public HoldingDatum HoldingDatum { get; set; }

        [DataMember]
        public int RaceNumber { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public int HorseCount { get; set; }

        [DataMember]
        public int CourseLength { get; set; }

        [DataMember]
        public CourseType CourseType { get; set; }

        [DataMember]
        public string Title { get; set; }


        public string NumberOfHeldAsUrlString => HoldingDatum.NumberOfHeld.ToString().PadLeft(2, '0');
        public string NumberOfDayAsUrlString => HoldingDatum.NumberOfDay.ToString().PadLeft(2, '0');
        public string RaceNumberAsUrlString => RaceNumber.ToString().PadLeft(2, '0');


        public RaceData(HoldingDatum holdingDatum, int raceNumber)
        {
            HoldingDatum = holdingDatum;
            RaceNumber = raceNumber;
            StartTime = HoldingDatum.StartTimeList[raceNumber - 1];
            HorseCount = HoldingDatum.HorseCountList[raceNumber - 1];
            CourseLength = HoldingDatum.CourseLengthList[raceNumber - 1];
            CourseType = HoldingDatum.CourseTypeList[raceNumber - 1];
            Title = HoldingDatum.TitleList[raceNumber - 1];
        }

        private string GetRaceIdString()
        {
            if (HoldingDatum.Region.RagionType == RegionType.Central)
            {
                return $"race_id={HoldingDatum.HeldDate.ToString("yyyy")}{HoldingDatum.Region.RegionId}{NumberOfHeldAsUrlString}{NumberOfDayAsUrlString}{RaceNumberAsUrlString}";
            }
            else
            {
                return $"race_id={HoldingDatum.HeldDate.ToString("yyyy")}{HoldingDatum.Region.RegionId}{HoldingDatum.HeldDate.ToString("MMdd")}{RaceNumberAsUrlString}";
            }
        }

        private string GetRealtimeIdString()
        {
            return $"i={HoldingDatum.HeldDate.ToString("yyyyMMdd")}{HoldingDatum.Region.RegionId}{RaceNumberAsUrlString}";
        }

        public string ToRealTimeOddsPageUrlString(TicketType ticketType)
        {
            var urlBase = "http://bachu.purasu.com/r/";

            if (ticketType == TicketType.Win || ticketType == TicketType.Place)
            {
                return $"{urlBase}{Utility.TicketTypeToRealTimeOddsUrlString[ticketType]}?{GetRealtimeIdString()}&os=n&us=1&it=n";
            }
            else
            {
                //単勝、複勝以外では人気順ページに行く
                return $"{urlBase}{Utility.TicketTypeToRealTimeOddsUrlString[ticketType]}?{GetRealtimeIdString()}";
            }
        }

        public string ToHorseInfoPageUrlString()
        {
            return $"https://race.netkeiba.com/race/shutuba.html?{GetRaceIdString()}&rf=race_submenu";
        }

        public string ToRaceOddsPageUrlString(TicketType ticketType)
        {
            var urlBase = HoldingDatum.Region.RagionType == RegionType.Regional ?
                        "https://nar.netkeiba.com/odds/index.html" :
                        "https://race.netkeiba.com/odds/index.html";

            if (ticketType == TicketType.Win || ticketType == TicketType.Place)
            {
                return $"{urlBase}?{GetRaceIdString()}&{Utility.TicketTypeToUrlString[ticketType]}&rf=shutuba_submenu";
            }
            else
            {
                //単勝以外では人気順ページに行く
                return $"{urlBase}?{GetRaceIdString()}&{Utility.TicketTypeToUrlString[ticketType]}&housiki=c99";
            }
        }

        public string ToNetKeibaIpatPageUrlString()
        {
            var urlBase = HoldingDatum.Region.RagionType == RegionType.Regional ?
            "https://nar.netkeiba.com/ipat/ipat.html" :
            "https://race.netkeiba.com/ipat/ipat.html";


            return $"{urlBase}?date={HoldingDatum.HeldDate.ToString("yyyyMMdd")}&{GetRaceIdString()}";
        }

        public string ToRaceResultPageUrlString()
        {
            var urlBase = HoldingDatum.Region.RagionType == RegionType.Regional ?
            "https://nar.netkeiba.com/race/result.html" :
            "https://race.netkeiba.com/race/result.html";

            return $"{urlBase}?{GetRaceIdString()}";
        }

        public string ToRaceListPageUrlString()
        {
            var urlBase = HoldingDatum.Region.RagionType == RegionType.Regional ?
                        "https://nar.netkeiba.com/odds/index.html" :
                        "https://race.netkeiba.com/odds/index.html";

            return $"{urlBase}?{GetRaceIdString()}";
        }

        public bool Equals(RaceData raceData)
        {
            return HoldingDatum.Equals(raceData.HoldingDatum) && RaceNumber == raceData.RaceNumber;
        }

        public override int GetHashCode()
        {
            return HoldingDatum.GetHashCode() ^ RaceNumber.GetHashCode();
        }
    }
}
