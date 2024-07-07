using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GreatUma.Utils;


namespace GreatUma.Models
{
    [DataContract]
    public class BetConfig
    {

        private List<BetConfigForTicketType> CurrentList => new List<BetConfigForTicketType>()
            {
                QuinellaBetConfig,
                WideBetConfig
            };

        [DataMember]
        public BetConfigForTicketType QuinellaBetConfig { get; set; } = new BetConfigForTicketType();

        [DataMember]
        public BetConfigForTicketType WideBetConfig { get; set; } = new BetConfigForTicketType();


        public bool ContainCentral()
        {
            return CurrentList.Any(_ => _.PurchaseCentral);
        }

        public bool ContainRegional()
        {
            return CurrentList.Any(_ => _.PurchaseRegional);
        }
    }

    [DataContract]

    public class BetConfigForTicketType
    {

        /// <summary>
        /// このオッズ比以上のチケットを購入する
        /// </summary>
        [DataMember]

        public double OddsRatio { get; set; } = 1.3;

        /// <summary>
        /// 最低払い戻し金額。払い戻しがこの金額以上になるようにオッズ額を修正する。
        /// </summary>
        [DataMember]
        public double MinimumPayBack { get; set; } = 3000;

        /// <summary>
        /// 購入しても良い最低オッズ
        /// </summary>
        [DataMember]
        public double MinimumOdds { get; set; } = 2;

        /// <summary>
        /// 購入しても良い最大オッズ
        /// </summary>
        [DataMember]
        public double MaximumOdds { get; set; } = 100;

        /// <summary>
        /// ココモ法を使うかどうか。
        /// </summary>
        [DataMember]
        public bool UseCocomo { get; set; } = true;

        /// <summary>
        /// ココモ法を使う場合に、何連敗ごとにオッズを増やすか
        /// </summary>
        [DataMember]
        public int CocomoThreshold { get; set; } = 50;

        /// <summary>
        /// ココモ法を使う場合に、許容可能な倍率（この倍率になったら加算をやめる）
        /// </summary>
        [DataMember]
        public int CocomoMaxMagnification { get; set; } = 5;

        /// <summary>
        /// 中央競馬を買うかどうか。
        /// </summary>
        [DataMember]
        public bool PurchaseCentral { get; set; } = true;

        /// <summary>
        /// 地方競馬を買うかどうか。
        /// </summary>
        [DataMember]
        public bool PurchaseRegional { get; set; } = false;

        /// <summary>
        /// 一度に購入できる最大チケット数（歪み降順）
        /// </summary>
        [DataMember]
        public int MaxPurchaseCountOrderByRatio { get; set; } = 1;

        /// <summary>
        /// 一度に購入できる最大チケット数（確率が高い順）
        /// </summary>
        [DataMember]
        public int MaxPurchaseCountOrderByProbability { get; set; } = 1;


        public bool NeedPurchase => PurchaseCentral || PurchaseRegional;
    }
}
