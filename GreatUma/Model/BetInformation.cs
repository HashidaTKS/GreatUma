using GreatUma.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GreatUma.Infrastructures;


namespace GreatUma.Models
{
    [DataContract]
    public class BetInformation
    {
        [DataMember]
        public RaceData RaceData { get; set; }

        [DataMember]
        public List<BetDatum> BetData { get; set; }

        public BetInformation()
        {
        }


        public BetInformation(RaceData raceData, List<BetDatum> betData)
        {
            RaceData = raceData;
            BetData = betData;
        }

        public BetInformationRepository GetRepository()
        {
            return GetRepository(RaceData);
        }

        public static BetInformationRepository GetRepository(RaceData raceData)
        {
            var path = Utility.GetBetInformationFilePath(raceData);
            return new BetInformationRepository(path);
        }
    }
}
