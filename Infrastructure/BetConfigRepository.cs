using System;
using GreatUma.Models;

namespace GreatUma.Infrastructures
{
    public class BetConfigRepository: BaseRepository<BetConfig>
    {
        public BetConfigRepository() : base("bet_config.xml")
        {

        }

        public BetConfigRepository(string filePath) : base(filePath)
        {

        }
    }
}
