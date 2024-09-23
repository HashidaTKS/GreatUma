using GreatUma.Utils;
using System.Collections.Generic;
using System.Linq;
using GreatUma.Models;
using GreatUma.Model;
using System.Security.Policy;
using NLog.Common;
using GreatUma.Infrastructure;

namespace GreatUma.Domain
{
    public class TargetSelector
    {
        private double TargetPlaceOdds { get; set; }
        private Scraper Scraper { get; set; }
        private DateTime TargetDate { get; set; }

        private WholeTargetConditionsRepository WholeTargetConditionsRepository { get; set; }


        public TargetSelector(Scraper scraper, DateTime targetDate, double targetPlaceOdds = 1.1)
        {
            this.Scraper = scraper;
            this.TargetDate = targetDate;
            this.TargetPlaceOdds = targetPlaceOdds;
        }
    }

}
