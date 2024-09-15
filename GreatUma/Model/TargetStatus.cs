using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GreatUma.Model
{
    public class TargetStatus
    {
        [DataMember]
        public List<HorseAndOddsCondition> HorseAndOddsConditionList { get; set; }
    }
}
