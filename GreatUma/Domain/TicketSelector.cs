using GreatUma.Utils;
using System.Collections.Generic;
using System.Linq;
using GreatUma.Models;

namespace GreatUma.Domain
{
    public class Cocomo
    {
        private decimal Before { get; set; } = 1;

        /// <summary>
        /// N回ココモを実行した場合の倍率を取得する
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private decimal GetMagnification(decimal n, decimal maxValue)
        {
            Reset();
            decimal current = 1;
            for (var i = 0; i < n; i++)
            {
                if (current > maxValue)
                {
                    return maxValue;
                }
                current = GetNext(current);
            }
            return current;
        }

        /// <summary>
        /// 次の倍率を取得する
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private decimal GetNext(decimal current)
        {
            var next = current + Before;
            Before = current;
            return next;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Reset()
        {
            Before = 1;
        }

        public int GetMagnification(BetConfigForTicketType betConfig, BetResultStatusOfTicketType betResultStatus)
        {
            var magnification = 1;
            if (betResultStatus.CountOfContinuationLose >= 1 && betConfig.CocomoThreshold > 0)
            {
                var division = betResultStatus.CountOfContinuationLose / betConfig.CocomoThreshold;
                magnification = (int)GetMagnification(division, (decimal)betConfig.CocomoMaxMagnification);
            }
            return magnification;
        }
    }

    /// <summary>
    /// 購入すべきチケットを選択する。
    /// </summary>
    public class TicketSelector
    {
        public static IEnumerable<BetDatum> SelectToBet(ActualRaceAndOddsData outputRaceData, BetConfig betconfig, BetResultStatus betResultStatus)
        {
            List<BetDatum> betData = new List<BetDatum>();
            var quinellaTicketList = SelectQuinellaTicket(outputRaceData, betconfig.QuinellaBetConfig, betResultStatus.QuinellaBetStatus).ToList();
            betData.AddRange(quinellaTicketList.OrderByDescending(_ => _.OddsRatio).Take(betconfig.QuinellaBetConfig.MaxPurchaseCountOrderByRatio));
            betData.AddRange(quinellaTicketList.OrderBy(_ => _.TheoreticalOdds).Take(betconfig.QuinellaBetConfig.MaxPurchaseCountOrderByProbability));

            var wideTicketList = SelectWideTicket(outputRaceData, betconfig.WideBetConfig, betResultStatus.WideBetStatus).ToList();
            betData.AddRange(wideTicketList.OrderByDescending(_ => _.OddsRatio).Take(betconfig.WideBetConfig.MaxPurchaseCountOrderByRatio));
            betData.AddRange(wideTicketList.OrderBy(_ => _.TheoreticalOdds).Take(betconfig.WideBetConfig.MaxPurchaseCountOrderByProbability));
            return betData.Distinct();
        }

        public static IEnumerable<BetDatum> SelectWinTicket(ActualRaceAndOddsData raceDataForComparison, BetConfigForTicketType betconfig, BetResultStatusOfTicketType betResultStatus)
        {
            return SelectTicketBase(raceDataForComparison, betconfig, betResultStatus, TicketType.Win);

        }

        public static IEnumerable<BetDatum> SelectWideTicket(ActualRaceAndOddsData raceDataForComparison, BetConfigForTicketType betconfig, BetResultStatusOfTicketType betResultStatus)
        {
            return SelectTicketBase(raceDataForComparison, betconfig, betResultStatus, TicketType.Wide);

        }

        public static IEnumerable<BetDatum> SelectQuinellaTicket(ActualRaceAndOddsData raceDataForComparison, BetConfigForTicketType betconfig, BetResultStatusOfTicketType betResultStatus)
        {
            return SelectTicketBase(raceDataForComparison, betconfig, betResultStatus, TicketType.Quinella);
        }

        public static IEnumerable<BetDatum> SelectExactaTicket(ActualRaceAndOddsData raceDataForComparison, BetConfigForTicketType betconfig, BetResultStatusOfTicketType betResultStatus)
        {
            return SelectTicketBase(raceDataForComparison, betconfig, betResultStatus, TicketType.Exacta);

        }

        public static IEnumerable<BetDatum> SelectTrifectaTicket(ActualRaceAndOddsData raceDataForComparison, BetConfigForTicketType betconfig, BetResultStatusOfTicketType betResultStatus)
        {
            return SelectTicketBase(raceDataForComparison, betconfig, betResultStatus, TicketType.Trifecta);

        }

        public static IEnumerable<BetDatum> SelectTrioTicket(ActualRaceAndOddsData raceDataForComparison, BetConfigForTicketType betconfig, BetResultStatusOfTicketType betResultStatus)
        {
            return SelectTicketBase(raceDataForComparison, betconfig, betResultStatus, TicketType.Trio);

        }

        private static double GetOddsRatio(double actual, double theoretical)
        {
            if (theoretical <= 0)
            {
                return 0;
            }
            return actual / theoretical;
        }

        /// <summary>
        /// 的中時の見積もり払戻額が指定金額以上になるようなベット額を取得する
        /// </summary>
        /// <returns></returns>
        private static int GetAdjustedBetMoney(double targetMoney, double odds)
        {
            if (odds <= 0)
            {
                //ありえないが
                return 100;
            }
            var ratio = targetMoney / (odds * 100);
            return (int)(ratio + 1) * 100;
        }

        private static IEnumerable<BetDatum> SelectTicketBase(ActualRaceAndOddsData raceData, BetConfigForTicketType betConfigForTicketType, BetResultStatusOfTicketType betResultStatusOfTicketType, TicketType ticketType)
        {
            if (raceData.BaseRaceData.HoldingDatum.Region.RagionType == RegionType.Central)
            {
                if (!betConfigForTicketType.PurchaseCentral)
                {
                    yield break;
                }
            }
            if (raceData.BaseRaceData.HoldingDatum.Region.RagionType == RegionType.Regional)
            {
                if (!betConfigForTicketType.PurchaseRegional)
                {
                    yield break;
                }
            }
            var actualOdds = raceData.GetOddsOfTicketType(ticketType);
            var count = actualOdds.Count;
            var cocomo = new Cocomo();
            for (var i = 0; i < count; i++)
            {
                var betMoney = GetAdjustedBetMoney(betConfigForTicketType.MinimumPayBack, actualOdds[i].LowOdds);
                if (betConfigForTicketType.UseCocomo)
                {
                    betMoney *= cocomo.GetMagnification(betConfigForTicketType, betResultStatusOfTicketType);
                }

                yield return new BetDatum(
                    raceData.BaseRaceData,
                    actualOdds[i].HorseData.Select(_ => _.Number).ToList(),
                    betMoney,
                    actualOdds[i].LowOdds,
                    actualOdds[i].LowOdds,
                    ticketType);
            }
        }
    }
}
