using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using GreatUma.Infrastructures;
using GreatUma.Models;

namespace GreatUma.Utils
{
    /// <summary>
    /// クラスを作るまでもないユーティリティ
    /// </summary>
    public static class Utility
    {
        private static readonly List<HoldingRegion> RegionInforomation = new List<HoldingRegion>()
        {
            //パフォーマンスが悪そうなら辞書化する
            new HoldingRegion("札幌", "01", RegionType.Central),
            new HoldingRegion("函館", "02", RegionType.Central),
            new HoldingRegion("福島", "03", RegionType.Central),
            new HoldingRegion("新潟", "04", RegionType.Central),
            new HoldingRegion("東京", "05", RegionType.Central),
            new HoldingRegion("中山", "06", RegionType.Central),
            new HoldingRegion("中京", "07", RegionType.Central),
            new HoldingRegion("京都", "08", RegionType.Central),
            new HoldingRegion("阪神", "09", RegionType.Central),
            new HoldingRegion("小倉", "10", RegionType.Central),
            new HoldingRegion("帯広", "65", RegionType.Regional),
            new HoldingRegion("門別", "30", RegionType.Regional),
            new HoldingRegion("盛岡", "35", RegionType.Regional),
            new HoldingRegion("水沢", "36", RegionType.Regional),
            new HoldingRegion("浦和", "42", RegionType.Regional),
            new HoldingRegion("船橋", "43", RegionType.Regional),
            new HoldingRegion("大井", "44", RegionType.Regional),
            new HoldingRegion("川崎", "45", RegionType.Regional),
            new HoldingRegion("金沢", "46", RegionType.Regional),
            new HoldingRegion("笠松", "47", RegionType.Regional),
            new HoldingRegion("名古屋", "48", RegionType.Regional),
            new HoldingRegion("園田", "50", RegionType.Regional),
            new HoldingRegion("姫路", "51", RegionType.Regional),
            new HoldingRegion("高知", "54", RegionType.Regional),
            new HoldingRegion("佐賀", "55", RegionType.Regional),
        };

        public static string GetRegionNameAtRakuten(HoldingRegion region)
        {
            if (region.RegionId == "65")
            {
                return "帯広ば";
            }
            else
            {
                return region.RegionName;
            }
        }

        public static HoldingRegion GetRegionFromName(string regionName)
        {
            //念のため部分一致にしておく。どうにかしたい。
            return RegionInforomation.FirstOrDefault(_ => regionName.Contains(_.RegionName));
        }

        public static HoldingRegion GetRegionFromId(string id)
        {
            return RegionInforomation.FirstOrDefault(_ => id == _.RegionId);
        }

        public static RegionType GetRegionType(string regionName)
        {
            return RegionInforomation.FirstOrDefault(_ => regionName.Contains(_.RegionName)).RagionType;
        }

        public static Dictionary<TicketType, string> TicketTypeToRealTimeOddsUrlString = new Dictionary<TicketType, string>
        {
            { TicketType.Win , "racet" },
            { TicketType.Place , "racef" },
            { TicketType.Exacta , "raceoddsrankut" },
            { TicketType.Quinella , "raceoddsranku" },
            { TicketType.Trifecta , "raceoddsrank3rt" },
            { TicketType.Trio , "raceoddsrank3rp" },
            { TicketType.Wide , "raceoddsrankwd" },
        };

        public static Dictionary<TicketType, string> TicketTypeToUrlString = new Dictionary<TicketType, string>
        {
            { TicketType.Win , "type=b1" },
            { TicketType.Place , "type=b1" },
            { TicketType.Exacta , "type=b6" },
            { TicketType.Quinella , "type=b4" },
            { TicketType.Trifecta , "type=b8" },
            { TicketType.Trio , "type=b7" },
            { TicketType.Wide , "type=b5" },
        };

        public static string GetRaceAndOddsDataFilePath(RaceData raceInfo, OddsType oddsType)
        {
            return Path.Combine(
                "RaceData",
                raceInfo.HoldingDatum.HeldDate.ToString("yyyyMMdd"),
                oddsType.ToString(),
                $"{raceInfo.HoldingDatum.Region.RegionId}-{raceInfo.HoldingDatum.Region.RegionName}-{raceInfo.RaceNumber}.xml");
        }

        public static string GetBetInformationFilePath(RaceData raceInfo)
        {
            return Path.Combine(
                "RaceData",
                raceInfo.HoldingDatum.HeldDate.ToString("yyyyMMdd"),
                "BetInfo",
                $"{raceInfo.HoldingDatum.Region.RegionId}-{raceInfo.HoldingDatum.Region.RegionName}-{raceInfo.RaceNumber}.xml");
        }

        public static Dictionary<TicketType, string> TicketTypeToRakutenBetTypeString = new Dictionary<TicketType, string>
        {
            { TicketType.Win, "単勝"},
            { TicketType.Place , "複勝" },
            { TicketType.Quinella, "馬複"},
            { TicketType.Exacta, "馬単"},
            { TicketType.Trio, "三連複"},
            { TicketType.Trifecta, "三連単"},
            { TicketType.Wide, "ワイド"},
        };

        public static Dictionary<TicketType, string> TicketTypeToJraBetTypeString = new Dictionary<TicketType, string>
        {
            { TicketType.Win, "単勝"},
            { TicketType.Place , "複勝" },
            { TicketType.Quinella, "馬連"},
            { TicketType.Exacta, "馬単"},
            { TicketType.Trio, "三連複"},
            { TicketType.Trifecta, "三連単"},
            { TicketType.Wide, "ワイド"},
        };

        public static double GetStatisticalOddsFromActualOdds(double odds, int rank)
        {
            return 1.0 / GetStatisticalProbabilityFromActualOdds(odds, rank);
        }

        public static double GetStatisticalProbabilityFromActualOdds(double odds, int rank)
        {
            if (odds == 1)
            {
                return 1;
            }
            if (odds <= 1.1)
            {
                return 0.793;
            }
            if (odds <= 1.2)
            {
                return 0.673;
            }
            if (odds <= 1.3)
            {
                return 0.628;
            }
            if (odds <= 1.4)
            {
                return 0.556;
            }
            if (odds <= 1.5)
            {
                return 0.52;
            }
            if (odds <= 1.6)
            {
                return 0.499;
            }
            if (odds <= 1.7)
            {
                return 0.441;
            }
            if (odds <= 1.8)
            {
                return 0.432;
            }
            if (odds <= 1.9)
            {
                return 0.400;
            }
            if (odds <= 2.0)
            {
                return 0.368;
            }
            if (odds <= 2.1)
            {
                return 0.354;
            }
            if (odds <= 2.2)
            {
                return 0.334;
            }
            if (odds <= 2.3)
            {
                return 0.329;
            }
            if (odds <= 2.4)
            {
                return 0.328;
            }
            if (odds <= 2.5)
            {
                return 0.317;
            }
            if (odds <= 2.6)
            {
                return 0.301;
            }
            if (odds <= 2.7)
            {
                return 0.292;
            }
            if (odds <= 2.8)
            {
                return 0.272;
            }
            if (odds <= 2.9)
            {
                return 0.279;
            }
            if (odds <= 3.4)
            {
                return 0.24;
            }
            if (odds <= 3.9)
            {
                if (rank == 1)
                {
                    return 0.211;
                }
                if (rank == 2)
                {
                    return 0.222;
                }
                return 0.22;
            }
            if (odds <= 4.9)
            {
                if (rank == 1)
                {
                    return 0.171;
                }
                if (rank == 2)
                {
                    return 0.178;
                }

                return 0.175;
            }
            if (odds <= 5.9)
            {
                return 0.151;
            }
            if (odds <= 6.9)
            {
                return 0.125;
            }
            if (odds <= 7.9)
            {
                return 0.11;
            }
            if (odds <= 8.9)
            {
                return 0.096;
            }
            if (odds <= 9.9)
            {
                return 0.09;
            }
            if (odds <= 14.9)
            {
                return 0.072;
            }
            if (odds <= 19.9)
            {
                return 0.05;
            }
            if (odds <= 24.9)
            {
                return 0.037;
            }
            if (odds <= 29.9)
            {
                return 0.031;
            }
            if (odds <= 39.9)
            {
                return 0.024;
            }
            if (odds <= 49.9)
            {
                return 0.018;
            }
            if (odds <= 59.9)
            {
                return 0.014;
            }
            if (odds <= 69.9)
            {
                return 0.011;
            }
            if (odds <= 79.9)
            {
                return 0.011;
            }
            if (odds <= 89.9)
            {
                return 0.009;
            }
            if (odds <= 99.9)
            {
                return 0.006;
            }
            return 0;

            //if (odds <= 150)
            //{
            //    return 0.005;
            //}
            //if (odds <= 200)
            //{
            //    return 0.003;
            //}
            //if (odds <= 500)
            //{
            //    return 0.001;
            //}
            ////勝ったことがない。。。
            //return 0;
        }

        /// <summary>
        /// 地方競馬全体から求めた勝率を使う。
        /// </summary>
        /// <param name="odds"></param>
        /// <returns></returns>
        public static double GetStatisticalProbabilityFromActualOddsForRegional(double odds)
        {
            if (odds == 1)
            {
                return 0.895;
            }
            if (odds <= 1.1)
            {
                return 0.81;
            }
            if (odds <= 1.2)
            {
                return 0.63;
            }
            if (odds <= 1.3)
            {
                return 0.67;
            }
            if (odds <= 1.4)
            {
                return 0.56;
            }
            if (odds <= 1.5)
            {
                return 0.5;
            }
            if (odds <= 1.6)
            {
                return 0.43;
            }
            if (odds <= 1.7)
            {
                return 0.43;
            }
            if (odds <= 1.8)
            {
                return 0.44;
            }
            if (odds <= 1.9)
            {
                return 0.41;
            }
            if (odds <= 2.0)
            {
                return 0.39;
            }
            if (odds <= 2.1)
            {
                return 0.356;
            }
            if (odds <= 2.2)
            {
                return 0.41;
            }
            if (odds <= 2.3)
            {
                return 0.38;
            }
            if (odds <= 2.4)
            {
                return 0.34;
            }
            if (odds <= 2.5)
            {
                return 0.317;
            }
            if (odds <= 2.6)
            {
                return 0.333;
            }
            if (odds <= 2.7)
            {
                return 0.227;
            }
            if (odds <= 2.8)
            {
                return 0.288;
            }
            if (odds <= 2.9)
            {
                return 0.32;
            }
            if (odds <= 3.4)
            {
                return 0.26;
            }
            if (odds <= 3.9)
            {
                return 0.213;
            }
            if (odds <= 4.9)
            {
                return 0.168;
            }
            if (odds <= 5.9)
            {
                return 0.156;
            }
            if (odds <= 6.9)
            {
                return 0.102;
            }
            if (odds <= 7.9)
            {
                return 0.106;
            }
            if (odds <= 8.9)
            {
                return 0.118;
            }
            if (odds <= 9.9)
            {
                return 0.084;
            }
            if (odds <= 14.9)
            {
                return 0.06;
            }
            if (odds <= 19.9)
            {
                return 0.05;
            }
            if (odds <= 24.9)
            {
                return 0.033;
            }
            if (odds <= 29.9)
            {
                return 0.0293;
            }
            if (odds <= 39.9)
            {
                return 0.0285;
            }
            if (odds <= 49.9)
            {
                return 0.0214;
            }
            if (odds <= 59.9)
            {
                return 0.0107;
            }
            if (odds <= 69.9)
            {
                return 0.009;
            }
            if (odds <= 79.9)
            {
                return 0.0105;
            }
            if (odds <= 89.9)
            {
                return 0.003;
            }
            if (odds <= 99.9)
            {
                return 0.005;
            }
            //これ以下の値は見てもしょうがないので。
            return 0;
            //if (odds <= 150)
            //{
            //    return 0.0027;
            //}
            //if (odds <= 200)
            //{
            //    return 0.00207;
            //}
            //if (odds <= 500)
            //{
            //    return 0.002;
            //}
            ////勝ったことがない。。。
            //return 0;
        }

        public static double GetGradeStatisticalProbabilityFromActualOdds(double odds, int rank)
        {
            if (odds == 1)
            {
                return 1;
            }
            if (odds <= 1.1)
            {
                return 0.905;
            }
            if (odds <= 1.2)
            {
                return 0.640;
            }
            if (odds <= 1.3)
            {
                return 0.532;
            }
            if (odds <= 1.4)
            {
                return 0.558;
            }
            if (odds <= 1.5)
            {
                //return 0.378;
                //件数が少ない故の下振れと思われるので補正
                return 0.5;
            }
            if (odds <= 1.6)
            {
                return 0.457;
            }
            if (odds <= 1.7)
            {
                return 0.42;
            }
            if (odds <= 1.8)
            {
                return 0.446;
            }
            if (odds <= 1.9)
            {
                return 0.363;
            }
            if (odds <= 2.0)
            {
                return 40.2;
            }
            if (odds <= 2.1)
            {
                return 0.337;
            }
            if (odds <= 2.2)
            {
                return 0.337;
            }
            if (odds <= 2.3)
            {
                return 0.355;
            }
            if (odds <= 2.4)
            {
                return 0.275;
            }
            if (odds <= 2.5)
            {
                return 0.293;
            }
            if (odds <= 2.6)
            {
                return 0.252;
            }
            if (odds <= 2.7)
            {
                return 0.281;
            }
            if (odds <= 2.8)
            {
                return 0.255;
            }
            if (odds <= 2.9)
            {
                return 0.287;
            }
            if (odds <= 3.4)
            {
                return 0.243;
            }
            if (odds <= 3.9)
            {
                return 0.233;
            }
            if (odds <= 4.9)
            {
                return 0.178;
            }
            if (odds <= 5.9)
            {
                return 0.159;
            }
            if (odds <= 6.9)
            {
                return 0.124;
            }
            if (odds <= 7.9)
            {
                return 0.11;
            }
            if (odds <= 8.9)
            {
                return 0.099;
            }
            if (odds <= 9.9)
            {
                return 0.092;
            }
            if (odds <= 14.9)
            {
                return 0.079;
            }
            if (odds <= 19.9)
            {
                return 0.05;
            }
            if (odds <= 24.9)
            {
                return 0.037;
            }
            if (odds <= 29.9)
            {
                return 0.031;
            }
            if (odds <= 39.9)
            {
                return 0.024;
            }
            if (odds <= 49.9)
            {
                return 0.018;
            }
            if (odds <= 59.9)
            {
                return 0.022;
            }
            if (odds <= 69.9)
            {
                return 0.011;
            }
            if (odds <= 79.9)
            {
                return 0.014;
            }
            if (odds <= 89.9)
            {
                return 0.004;
            }
            if (odds <= 99.9)
            {
                return 0.008;
            }
            //これ以下の値は見てもしょうがないので。
            return 0;
            //if (odds <= 150)
            //{
            //    return 0.004;
            //}
            //if (odds <= 200)
            //{
            //    return 0.005;
            //}
            //if (odds <= 500)
            //{
            //    return 0.001;
            //}
            ////勝ったことがない。。。
            //return 0;
        }
    }
}
