using GreatUma.Models;
using GreatUma.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatUma.Domain
{
    public class ResultOfBet
    {
        public BetDatum BetDatum { get; set; }

        public RaceResult RaceResult { get; set; }

        public double BetMoney => BetDatum.BetMoney;

        public double PayBack { get; set; }

        public bool IsHit => PayBack > 0;

        public double Profit => PayBack - BetMoney;

        public ResultOfBet(BetDatum betDatum, RaceResult raceResult)
        {
            BetDatum = betDatum;
            RaceResult = raceResult;
            PayBack = GetPayBack();
        }

        private double GetPayBack()
        {
            var results = RaceResult.GetResultHorseAndPayoutOfTicket(BetDatum.TicketType);

            //Todo: 綺麗にする
            if (BetDatum.TicketType == TicketType.Quinella || BetDatum.TicketType == TicketType.Wide || BetDatum.TicketType == TicketType.Trio)
            {
                foreach (var result in results)
                {
                    //元々並びは同じはずだが、一応
                    if (result.Item1.OrderBy(_ => _).SequenceEqual(BetDatum.HorseNumList.OrderBy(_ => _)))
                    {
                        return result.Item2 * (BetMoney / 100);
                    }
                }
            }
            else
            {
                foreach (var result in results)
                {
                    if (result.Item1.SequenceEqual(BetDatum.HorseNumList))
                    {
                        return result.Item2 * (BetMoney / 100);
                    }
                }
            }
            return 0.0;
        }

        public static string GetCsvHeader()
        {
            return "開催日,開催地,レース番号,券種,馬,ベット額,払い戻し,利益";
        }

        public string ToCsv()
        {
            //Todo: Use Csv Helper
            return $"{BetDatum.RaceData.HoldingDatum.HeldDate.ToString("yyyy/MM/dd")},{BetDatum.RaceData.HoldingDatum.Region.RegionName},{BetDatum.RaceData.RaceNumber},{BetDatum.TicketType.ToString()},{string.Join(" - ", BetDatum.HorseNumList)},{BetMoney},{PayBack},{Profit}";
        }
    }
}
