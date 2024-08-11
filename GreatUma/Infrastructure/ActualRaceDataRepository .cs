using GreatUma.Models;

namespace GreatUma.Infrastructures
{
    public class ActualRaceDataRepository : BaseRepository<ActualRaceAndOddsData>
    {
        public ActualRaceDataRepository(string raceDataRepository) : base(raceDataRepository)
        {
        }
    }
}
