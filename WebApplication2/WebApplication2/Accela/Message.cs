using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SOWebService.Accela
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public Result result { get; set; }
        [DataMember]
        public Result code { get; set; }
        [DataMember]
        public Result message { get; set; }
    }

    [DataContract]
    public class Result
    {
        [DataMember]
        public string Updates { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Success { get; set; }
    }
}