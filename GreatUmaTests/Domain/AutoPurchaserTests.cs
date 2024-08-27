using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreatUma.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreatUma.Models;
using System.Text.Json;
using System.Text.Json;


namespace GreatUma.Domain.Tests
{
    [TestClass()]
    public class AutoPurchaserTests
    {
        [TestMethod()]
        public void PurchaseTest()
        {
            var raceData = new RaceData(
                new HoldingDatum(
                    new HoldingRegion("福島", "03", Utils.RegionType.Central),
                    2,
                    3,
                    new DateTime(2024, 7, 6),
                    new List<DateTime>()
                    {
                              new DateTime(2024,7,6,10,10,00),
                              new DateTime(2024,7,6,10,45,00),
                              new DateTime(2024,7,6,11,15,00),
                              new DateTime(2024,7,6,11,45,00),
                              new DateTime(2024,7,6,12,35,00),
                              new DateTime(2024,7,6,13,5,00),
                              new DateTime(2024,7,6,13,35,00),
                              new DateTime(2024,7,6,14,5,00),
                              new DateTime(2024,7,6,14,36,00),
                              new DateTime(2024,7,6,15,11,00),
                              new DateTime(2024,7,6,15,45,00),
                              new DateTime(2024,7,6,16,30,00)
                    },
                    new List<int>()
                    {
                              8,
                              9,
                              15,
                              16,
                              14,
                              12,
                              15,
                              11,
                              9,
                              8,
                              15,
                              16
                    },
                    null,
                    null),
                1);
            var betDatum = new BetDatum(raceData, new List<int>() { 1 }, 100, 1.1, 1.1, Utils.TicketType.Win);
            //// テスト時は実際にログイン情報を指定する。
            //// ログイン情報はコミットしないように注意。
            var loginConfig = new LoginConfig()
            {

            };
            var purchaser = new AutoPurchaser(loginConfig);
            purchaser.Purchase(new List<BetDatum>() { betDatum });
            //Assert.Fail();
        }
    }
}