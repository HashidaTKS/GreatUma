using System;
using System.Linq;
using GreatUma.Models;


namespace GreatUma.Infrastructures
{
    public class HoldingInformationRepository : BaseRepository<HoldingInformation> { 

        public HoldingInformationRepository() : base("holding_info.xml")
        {
        }

        public HoldingDatum ReadFromDateAndRegion(DateTime date , GreatUma.Models.HoldingRegion region)
        {
            lock (this)
            {
                return ReadAll()?.HoldingData?.FirstOrDefault(_ => _.HeldDate.Date == date.Date && _.Region.RegionId == region.RegionId);
            }
        }
    }
}
