using GreatUma.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GreatUma.Model
{
    public class TargetConditionsOfDay
    {
        public DateTime TargetDate { get; set; }
        public List<TargetCondition> TargetConditionList { get; set; } = new List<TargetCondition>();
    }
}
