using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using GreatUma.Utils;


namespace GreatUma.Models
{
    [DataContract]
    [Serializable]
    public struct HoldingRegion
    {
        /// <summary>
        /// 地域名
        /// </summary>
        [DataMember]
        public string RegionName { get; private set; }

        /// <summary>
        /// その地域のUrlでのコースID
        /// </summary>
        [DataMember]
        public string RegionId { get; private set; }

        /// <summary>
        /// 中央か地方か
        /// </summary>
        [DataMember]
        public RegionType RagionType{ get; private set; } 

        public HoldingRegion(string regionName, string regionId, RegionType regionType)
        {
            RegionName = regionName;
            RegionId = regionId;
            RagionType = regionType;
        }
    }
}
