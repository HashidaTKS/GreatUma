using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GreatUma.Models
{
    [DataContract]
    [Serializable]
    public class OddsDatum : IEquatable<OddsDatum>
    {
        [DataMember]
        public List<HorseDatum> HorseData  { get; set; }

        [DataMember]
        ///複勝、ワイドの場合は低い方のオッズ
        ///そうでない場合はオッズ自体。
        public double LowOdds { get; set; }

        [DataMember]
        ///複勝、ワイドの場合は高い方のオッズ
        ///そうでない場合はオッズ自体。
        public double HighOdds { get; set; }


        public OddsDatum(IEnumerable<HorseDatum> horseData, double lowOdds, double highOdds)
        {
            HorseData = horseData.ToList();
            LowOdds = lowOdds;
            HighOdds = highOdds;
        }

        public string GetHorsesString()
        {
            return string.Join(" - ", HorseData.Select(_ => _.Number));
        }

        public string GetOddsString()
        {
            return $"{LowOdds:F1} - {HighOdds:F1}";
        }

        //厳密には連複系のケースが考慮できていない。
        //ただし、現状は連複系も馬番順に並ぶので大丈夫。
        public bool Equals(OddsDatum other)
        {
            if(other == null)
            {
                return false;
            }
            return other.HorseData.SequenceEqual(HorseData) && other.LowOdds == LowOdds && other.HighOdds == HighOdds;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public override int GetHashCode()
        {
            return HorseData.Select(_ => _.GetHashCode()).Aggregate(LowOdds.GetHashCode() ^ HighOdds.GetHashCode(), (a, b) => a ^ b);
        }
    }
}
