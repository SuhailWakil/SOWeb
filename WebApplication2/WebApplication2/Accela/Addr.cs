using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SOWebService.Accela
{
    [DataContract]
    public class Addr
    {
        [DataMember]
        public string addrRefNumber { get; set; }

        [DataMember]
        public string fullAddress { get; set; }

        [DataMember]
        public string jurisdiction_code { get; set; }

    }
}