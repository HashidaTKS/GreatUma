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

        [TestMethod()]
        public void GetHorseInfoTest()
        {
            var scraper = new Scraper();
            var date = new DateTime(2024, 7, 6);
            var acrual = scraper.GetHoldingInformation(date, Utils.RegionType.Central);
            Assert.AreEqual(3, acrual.HoldingData.Count);
            Assert.AreEqual(new HoldingRegion("福島", "03", Utils.RegionType.Central), acrual.HoldingData[0].Region);
            var raceData = new RaceData(acrual.HoldingData[0], 1);
            var horseInfoList = scraper.GetHorseInfo(raceData);
            Assert.AreEqual(8, horseInfoList.Count);
            Assert.AreEqual(new HorseDatum(1, "ビップジェシー", "菊沢"), horseInfoList[0]);
            Assert.AreEqual(new HorseDatum(8, "マイディアホープ", "石田"), horseInfoList[7]);

        }

        [TestMethod()]
        public void GetWinOddsTest()
        {
            var scraper = new Scraper();
            var date = new DateTime(2024, 7, 6);
            var acrual = scraper.GetHoldingInformation(date, Utils.RegionType.Central);
            Assert.AreEqual(3, acrual.HoldingData.Count);
            Assert.AreEqual(new HoldingRegion("福島", "03", Utils.RegionType.Central), acrual.HoldingData[0].Region);
            var raceData = new RaceData(acrual.HoldingData[0], 1);
            var winResultList = scraper.GetOdds(raceData, Utils.TicketType.Win);
            Assert.AreEqual(8, winResultList.Count);
            Assert.AreEqual(59.9, winResultList[0].LowOdds);
        }

        [TestMethod()]
        public void GetPlaceOddsTest()
        {
            var scraper = new Scraper();
            var date = new DateTime(2024, 7, 6);
            var acrual = scraper.GetHoldingInformation(date, Utils.RegionType.Central);
            Assert.AreEqual(3, acrual.HoldingData.Count);
            Assert.AreEqual(new HoldingRegion("福島", "03", Utils.RegionType.Central), acrual.HoldingData[0].Region);
            var raceData = new RaceData(acrual.HoldingData[0], 1);
            var winResultList = scraper.GetOdds(raceData, Utils.TicketType.Place);
            Assert.AreEqual(8, winResultList.Count);
            Assert.AreEqual(4.3, winResultList[0].LowOdds);
            Assert.AreEqual(26.1, winResultList[0].HighOdds);
        }

        [TestMethod()]
        public void GetRealTimeWinOddsTest()
        {
            var scraper = new Scraper();
            var date = new DateTime(2024, 7, 6);
            var acrual = scraper.GetHoldingInformation(date, Utils.RegionType.Central);
            Assert.AreEqual(3, acrual.HoldingData.Count);
            Assert.AreEqual(new HoldingRegion("福島", "03", Utils.RegionType.Central), acrual.HoldingData[0].Region);
            var raceData = new RaceData(acrual.HoldingData[0], 1);
            var winResultList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Win);
            Assert.AreEqual(8, winResultList.Count);
            Assert.AreEqual(56, winResultList[0].LowOdds);
        }

        [TestMethod()]
        public void GetRealTimePlaceOddsTest()
        {
            var scraper = new Scraper();
            var date = new DateTime(2024, 7, 6);
            var acrual = scraper.GetHoldingInformation(date, Utils.RegionType.Central);
            Assert.AreEqual(3, acrual.HoldingData.Count);
            Assert.AreEqual(new HoldingRegion("福島", "03", Utils.RegionType.Central), acrual.HoldingData[0].Region);
            var raceData = new RaceData(acrual.HoldingData[0], 1);
            var winResultList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Place);
            Assert.AreEqual(8, winResultList.Count);
            Assert.AreEqual(3.7, winResultList[0].LowOdds);
            Assert.AreEqual(20.4, winResultList[0].HighOdds);
        }
    }
}