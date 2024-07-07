using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreatUma.Models;


namespace GreatUma.Infrastructures
{
    public class BetInformationRepository : BaseRepository<BetInformation>
    {
        public BetInformationRepository(string betInformationPath) : base(betInformationPath)
        {
        }
    }
}
