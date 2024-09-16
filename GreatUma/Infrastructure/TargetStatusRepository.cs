using GreatUma.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreatUma.Model;

namespace GreatUma.Infrastructure
{
    public class TargetConfigRepository : BaseRepository<TargetConfig>
    {
        public TargetConfigRepository() : base("target_config.xml")
        {
        }

        public TargetConfigRepository(string filePath) : base(filePath)
        {

        }
    }
}
