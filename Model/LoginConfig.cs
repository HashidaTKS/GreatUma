using System;
using System.Runtime.Serialization;

namespace GreatUma.Models
{
    [DataContract]
    [Serializable]
    public class LoginConfig
    {
        [DataMember]
        public string NetkeibaId { get; set; }

        [DataMember]
        public string NetkeibaPassword { get; set; }

        [DataMember]
        public string RakutenId { get; set; }

        [DataMember]
        public string RakutenPassword { get; set; }

        [DataMember]
        public string JRA_SubscriberNumber { get; set; }

        [DataMember]
        public string JRA_P_ARS { get; set; }

        [DataMember]
        public string JRA_INET_ID { get; set; }

        [DataMember]
        public string JRA_LoginPassword { get; set; }

        public bool IsInitialized()
        {
            return !(string.IsNullOrEmpty(NetkeibaId) ||
                string.IsNullOrEmpty(NetkeibaPassword) ||
                string.IsNullOrEmpty(RakutenId) ||
                string.IsNullOrEmpty(RakutenPassword) ||
                string.IsNullOrEmpty(JRA_SubscriberNumber) ||
                string.IsNullOrEmpty(JRA_P_ARS) ||
                string.IsNullOrEmpty(JRA_INET_ID) ||
                string.IsNullOrEmpty(JRA_LoginPassword));
        }
    }
}
