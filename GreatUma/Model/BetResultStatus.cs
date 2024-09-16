using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using GreatUma.Utils;


namespace GreatUma.Models
{
    [DataContract]
    public class BetResultStatus
    {
        /// <summary>
        /// ステータスのチェックが行われている時刻
        /// チェックをした時刻ではなく、ここまでのデータはチェック済みである、という意味
        /// </summary>
        [DataMember]
        public DateTime CheckedTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 馬連のステータス
        /// </summary>
        [DataMember]
        public BetResultStatusOfTicketType QuinellaBetStatus { get; set; } = new BetResultStatusOfTicketType();
        /// <summary>
        /// ワイドのステータス
        /// </summary>
        [DataMember]
        public BetResultStatusOfTicketType WideBetStatus { get; set; } = new BetResultStatusOfTicketType();


        public BetResultStatusOfTicketType GetTicketTypeStatus(TicketType ticketType)
        {
            switch (ticketType)
            {
                default:
                case TicketType.Quinella:
                    return QuinellaBetStatus;
                case TicketType.Wide:
                    return WideBetStatus;
            }
        }
    }

    [DataContract]
    public class BetResultStatusOfTicketType
    {
        [DataMember]
        public int CountOfContinuationLose { get; set; } = 0;
    }
}
