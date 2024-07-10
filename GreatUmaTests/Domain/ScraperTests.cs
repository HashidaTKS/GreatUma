using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreatUma.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreatUma.Models;

namespace GreatUma.Domain.Tests
{
    [TestClass()]
    public class ScraperTests
    {
        [TestMethod()]
        public void GetHoldingInformationTest()
        {
            var scraper = new Scraper();
            var date = new DateTime(2024, 7, 6);
            var acrual = scraper.GetHoldingInformation(date, Utils.RegionType.Central);
            Assert.AreEqual(3, acrual.HoldingData.Count);
            Assert.AreEqual(new HoldingRegion("福島", "03", Utils.RegionType.Central), acrual.HoldingData[0].Region);
            Assert.AreEqual(new HoldingRegion("小倉", "10", Utils.RegionType.Central), acrual.HoldingData[1].Region);
            Assert.AreEqual(new HoldingRegion("函館", "02", Utils.RegionType.Central), acrual.HoldingData[2].Region);
        }
    }
}