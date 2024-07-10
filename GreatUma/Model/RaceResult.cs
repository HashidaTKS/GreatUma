using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using GreatUma.Utils;
using GreatUma.Infrastructures;


namespace GreatUma.Models
{
    [Serializable]
    [DataContract]
    public class RaceResult
    {
        //TODO:馬とPayoutの組み合わせをクラス化する

        [DataMember]
        public RaceData RaceData { get; set; }

        [DataMember]
        public double WinPayout { get; set; }

        [DataMember]
        public List<double> WidePayoutList { get; set; }

        [DataMember]
        public double QuinellaPayout { get; set; }

        [DataMember]
        public double ExtractPayout { get; set; }

        [DataMember]
        public double TrioPayout { get; set; }

        [DataMember]
        public double TrifectaPayout { get; set; }

        [DataMember]
        public List<int> WinHorse { get; set; }

        [DataMember]
        public List<List<int>> WideHorseList { get; set; }

        [DataMember]
        public List<int> QuinellaHorseList { get; set; }

        [DataMember]
        public List<int> ExtractHorseList { get; set; }

        [DataMember]
        public List<int> TrioHorseList { get; set; }

        [DataMember]
        public List<int> TrifectaHorseList { get; set; }

        public RaceResult()
        {
        }

        public RaceResult(RaceData raceData)
        {
            RaceData = raceData;
        }

        public IEnumerable<(List<int>, double)> GetResultHorseAndPayoutOfTicket(TicketType ticketType)
        {
            switch (ticketType)
            {
                default:
                case TicketType.Win:
                    yield return (WinHorse, WinPayout);
                    yield break;
                case TicketType.Quinella:
                    yield return(QuinellaHorseList, QuinellaPayout);
                    yield break;
                case TicketType.Exacta:
                    yield return(ExtractHorseList, ExtractPayout);
                    yield break;
                case TicketType.Trio:
                    yield return(TrioHorseList, TrioPayout);
                    yield break;
                case TicketType.Trifecta:
                    yield return(TrifectaHorseList, TrifectaPayout);
                    yield break;
                case TicketType.Wide:
                    var count = WideHorseList?.Count ?? 0;
                    for(var i = 0; i < count; i++)
                    {
                        yield return (WideHorseList[i], WidePayoutList[i]);
                    }
                    yield break;
            }
        }

        public RaceResultRepository GetRepository()
        {
            return GetRepository(RaceData);
        }

        public static RaceResultRepository GetRepository(RaceData raceData)
        {
            var path = Path.Combine(
            "RaceData",
            raceData.HoldingDatum.HeldDate.ToString("yyyyMMdd"),
            "ResultData",
            $"{raceData.HoldingDatum.Region.RegionId}-{raceData.HoldingDatum.Region.RegionName}-{raceData.RaceNumber}.xml");
            return new RaceResultRepository(path);
        }
    }
}
