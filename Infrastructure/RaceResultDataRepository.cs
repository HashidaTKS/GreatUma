using GreatUma.Models;

namespace GreatUma.Infrastructures
{
    public class RaceResultRepository : BaseRepository<RaceResult>
    {
        public RaceResultRepository(string raceResultPath) : base(raceResultPath)
        {
        }
    }
}
