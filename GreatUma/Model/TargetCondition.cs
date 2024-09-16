using GreatUma.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GreatUma.Model
{
    public class TargetCondition
    {
        [DisplayName("発送時刻")]
        public DateTime StartTime => RaceData?.StartTime ?? DateTime.MinValue;
        [DisplayName("競馬場")]
        public string Region => RaceData?.HoldingDatum.Region.RegionName ?? "";
        [DisplayName("タイトル")]
        public string Title => RaceData?.Title ?? "";
        [DisplayName("コース")]
        public string Course => RaceData?.CourseType.ToString() ?? "";
        [DisplayName("馬番")]
        public string HorseNum => CurrentWinOdds?.HorseData[0].Number.ToString() ?? "";
        [DisplayName("騎手")]
        public string Jockey => CurrentWinOdds?.HorseData[0].Jockey?.ToString() ?? "";
        [DisplayName("0時オッズ")]
        public string MidnightOdds => CurrentWinOdds == null || MidnightPlaceOdds == null ?
            "" :
            $"単勝:{MidnightWinOdds.LowOdds} 複勝:{MidnightPlaceOdds.LowOdds} - {MidnightPlaceOdds.HighOdds}";
        [DisplayName("現在オッズ")]
        public string CurrentOdds => CurrentWinOdds == null || CurrentPlaceOdds == null ?
            "" :
            $"単勝:{CurrentWinOdds.LowOdds} 複勝:{CurrentPlaceOdds.LowOdds} - {CurrentPlaceOdds.HighOdds}";
        [DisplayName("購入オッズ条件（3分前にこのオッズ以上なら購入）")]
        [DataMember]
        public double PurchaseOdds { get; set; }
        [Browsable(false)]
        [DataMember]
        public RaceData RaceData { get; set; }
        /// <summary>
        /// 0時時点の単勝オッズ。馬などの詳細なデータが必要な場合はこちらを使う。
        /// 実際には、0時時点ではなく、最初にこのクラスが作成された際のオッズであることに注意。
        /// </summary>
        [Browsable(false)]
        [DataMember]
        public OddsDatum MidnightWinOdds { get; set; }
        /// <summary>
        /// 0時時点の複勝オッズ。馬などの詳細なデータが必要な場合はこちらを使う。
        /// 実際には、0時時点ではなく、最初にこのクラスが作成された際のオッズであることに注意。
        /// </summary>
        [Browsable(false)]
        [DataMember]
        public OddsDatum MidnightPlaceOdds { get; set; }
        /// <summary>
        /// 現在の単勝オッズ。Midnightの方と比べ、馬などの詳細なデータが入っていない可能性があることに注意。
        /// 馬などの詳細なデータが必要な場合はMidnightWinOddsの方を使う。
        /// </summary>
        [Browsable(false)]
        [DataMember]
        public OddsDatum CurrentWinOdds { get; set; }
        /// <summary>
        /// 現在の複勝オッズ。Midnightの方と比べ、馬などの詳細なデータが入っていない可能性があることに注意。
        /// 馬などの詳細なデータが必要な場合はMidnightPlaceOddsの方を使う。
        /// </summary>
        [Browsable(false)]
        [DataMember]
        public OddsDatum CurrentPlaceOdds { get; set; }
    }
}
