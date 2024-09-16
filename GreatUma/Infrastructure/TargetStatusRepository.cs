using GreatUma.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreatUma.Model;

namespace GreatUma.Infrastructure
{
    public class TargetStatusRepository : BaseRepository<TargetStatus>
    {
        public TargetStatusRepository() : base("target_status.xml")
        {
        }

        public TargetStatusRepository(string filePath) : base(filePath)
        {

        }
    }
}
