using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GreatUma.Utils;


namespace GreatUma.Models
{
    [DataContract]
    [Serializable]
    public abstract class RaceAndOddsData
    {
        [DataMember]
        public RaceData BaseRaceData { get; set; }

        [DataMember]
        public List<OddsDatum> WinOdds { get; set; }

        [DataMember]
        public List<OddsDatum> ExactaOdds { get; set; }

        [DataMember]
        public List<OddsDatum> TrifectaOdds { get; set; }

        [DataMember]
        public List<OddsDatum> QuinellaOdds { get; set; }

        [DataMember]
        public List<OddsDatum> TrioOdds { get; set; }

        //実オッズの場合、最も低いオッズを表す
        //理論オッズの場合、的中確率を表す
        [DataMember]
        public List<OddsDatum> WideOdds { get; set; }

        [DataMember]
        public DateTime LastUpadateUtcTime;

        public RaceAndOddsData()
        {
        }

        public RaceAndOddsData(RaceData raceData)
        {
            BaseRaceData = raceData;
        }

        public List<OddsDatum> GetOddsOfTicketType(TicketType ticketType)
        {
            switch (ticketType)
            {
                default:
                case TicketType.Win:
                    return WinOdds;
                case TicketType.Exacta:
                    return ExactaOdds;
                case TicketType.Quinella:
                    return QuinellaOdds;
                case TicketType.Trifecta:
                    return TrifectaOdds;
                case TicketType.Trio:
                    return TrioOdds;
                case TicketType.Wide:
                    return WideOdds;
            }
        }
    }
}
