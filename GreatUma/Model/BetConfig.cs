using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GreatUma.Utils;


namespace GreatUma.Models
{
    [DataContract]
    public class BetConfig
    {
        public double WinOdds { get; set; }
    }
}
