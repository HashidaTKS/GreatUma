﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GreatUma.Models
{
    [DataContract]
    [Serializable]
    public class HorseDatum : IEquatable<HorseDatum>
    {
        [DataMember]
        public int Number { get; set; }
        [DataMember]
        public int Bracket { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Jockey { get; set; }

        public HorseDatum()
        {
        }

        public HorseDatum(int number, int bracket ,string name, string jockey)
        {
            Number = number;
            Bracket = bracket;
            Name = name;
            Jockey = jockey;
        }

        public override string ToString()
        {
            return $"馬番[{Number}], 名前[{Name}], 騎手[{Jockey}]";
        }

        public bool Equals(HorseDatum other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Number == Number && other.Name == Name && other.Jockey == Jockey;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode() ^ (Name?.GetHashCode() ?? 0) ^ (Jockey?.GetHashCode() ?? 0);
        }

    }
}
