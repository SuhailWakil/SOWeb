using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace SOWebService.Accela
{
    [DataContract]
    public class SOResult
    {
        [DataMember]
        public string token { get; set; }

        [DataMember]
        public string AltID { get; set; }

        [DataMember]
        public string RecID { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}