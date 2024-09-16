using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GreatUma.Model
{
    /// <summary>
    /// TODO: 設定の側面もあるので、ステータスと呼ぶべきかどうか再検討。
    /// </summary>
    public class TargetStatus
    {
        [DataMember]
        public int PurchasePrice { get; set; } = 0;
        [DataMember]
        public List<HorseAndOddsCondition> HorseAndOddsConditionList { get; set; }
    }
}
