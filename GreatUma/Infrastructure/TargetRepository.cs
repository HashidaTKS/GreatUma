using GreatUma.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreatUma.Model;

namespace GreatUma.Infrastructure
{
    public class WholeTargetConditionsRepository : BaseRepository<WholeTargetConditions>
    {
        public WholeTargetConditionsRepository() : base("whole_target_confitions.xml")
        {
        }

        public WholeTargetConditionsRepository(string filePath) : base(filePath)
        {

        }
    }
}
