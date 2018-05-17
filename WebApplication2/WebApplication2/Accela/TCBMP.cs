using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SOWebService.Accela
{
    [DataContract]
    public class TCBMP
    {
        [DataMember]
        public string token { get; set; }

        [DataMember]
        public string valid { get; set; }

        [DataMember]
        public string recordId { get; set; }

        [DataMember]
        public string inspId { get; set; }

        [DataMember]
        public string status { get; set; }

        [DataMember]
        public string appTypeAlias { get; set; }

        [DataMember]
        public string spcText { get; set; }

        [DataMember]
        public List<AccelaContact> contacts { get; set; }

        [DataMember]
        public string Message { get; set; }

    }

    [DataContract]
    public class AccelaContact
    {
        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string fullName { get; set; }

        [DataMember]
        public string firstName { get; set; }

        [DataMember]
        public string middleName { get; set; }

        [DataMember]
        public string lastName { get; set; }
    }

}