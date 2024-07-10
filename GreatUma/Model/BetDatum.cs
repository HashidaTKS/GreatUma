using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using GreatUma.Utils;


namespace GreatUma.Models
{
    [DataContract]
    public class BetDatum 
    {
        public RaceData RaceData { get; set; }

        [DataMember]
        public List<int> HorseNumList { get; set; }
        [DataMember]
        public int BetMoney { get; set; }
        [DataMember]
        public TicketType TicketType { get; set; }
        [DataMember]
        public double ActualOdds { get; set; }
        [DataMember]
        public double TheoreticalOdds { get; set; }

        public double OddsRatio => TheoreticalOdds > 0 ? ActualOdds / TheoreticalOdds : Int32.MaxValue;
        
        public BetDatum(RaceData raceData, List<int> horseNumList, int betYen, double actualOdds, double theoreticalOdds, TicketType ticketType)
        {
            RaceData = raceData;
            HorseNumList = horseNumList;
            BetMoney = betYen;
            ActualOdds = actualOdds;
            TheoreticalOdds = theoreticalOdds;
            TicketType = ticketType;
        }
    }
}
