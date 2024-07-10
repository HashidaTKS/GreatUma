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
        public double Odds { get; set; }

        public OddsDatum(IEnumerable<HorseDatum> horseData, double odds)
        {
            HorseData = horseData.ToList();
            Odds = odds;
        }

        public string GetHorsesString()
        {
            return string.Join(" - ", HorseData.Select(_ => _.Number));
        }

        public string GetOddsString()
        {
            return $"{Odds:F1}";
        }

        //厳密には連複系のケースが考慮できていない。
        //ただし、現状は連複系も馬番順に並ぶので大丈夫。
        public bool Equals(OddsDatum other)
        {
            if(other == null)
            {
                return false;
            }
            return other.HorseData.SequenceEqual(HorseData) && other.Odds == Odds;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public override int GetHashCode()
        {
            return HorseData.Select(_ => _.GetHashCode()).Aggregate(Odds.GetHashCode(), (a, b) => a ^ b);
        }
    }
}
