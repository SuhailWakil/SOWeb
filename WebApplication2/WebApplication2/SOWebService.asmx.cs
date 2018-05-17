using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SOWebService.Accela;
using System.Drawing;
using System.Web.Configuration;

namespace SOWebService
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]

    public class SOWebService : System.Web.Services.WebService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [WebMethod]
        public string CaptchaValidate(String EncodedResponse)
        {
            try
            {
                return ReCaptchaClass.Validate(EncodedResponse);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            return "";
        }

        [WebMethod]
        public SOResult SetDetailsRest(String problemCategory, String problemSubcategory,
            String otherDetails, String customerComments, String firstName, String lastName, String email, String phone1, String address, String city,
            String xcoordinate, String ycoordinate, String resolvedAddress, String locality, String neighborhood, String postal_code, String route, 
            String street_number, String image)
        {
            string[] inputs = { problemCategory,  problemSubcategory,
             customerComments,  firstName,  lastName,  email,  phone1, resolvedAddress};
            if (inputs.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                SOResult soresult = new SOResult();
                var message = "Required Fields Missing. Fields required are problemCategory,  problemSubcategory, customerComments,  firstName,  lastName,  email,  phone1, resolvedAddress";
                soresult.Message = message;
                log.Info(message);
                return soresult;
            }
            
            var leamsEmv = WebConfigurationManager.AppSettings["leamsEmv"];
            String result = AccelaRestServiceSO.AuthenticateRestUser("LEAMSDPW", leamsEmv);
            log.Info("In SetDetailsRest parameter: " + problemCategory +" - "+ problemSubcategory + " - " + otherDetails + " - " + customerComments + " - " + firstName + " - " + lastName + " - " + email + " - " + phone1 + " - " + address + " - " + city + " - " + xcoordinate + " - " + ycoordinate + " - " + resolvedAddress + " - " + locality + " - " + neighborhood + " - " + postal_code + " - " + route + " - " + street_number);
            var token = result;
            log.Info("In SetDetailsRest token: " + token);
            try
            {
                var re = AccelaRestServiceSO.SetDetailsRest(token, problemCategory, problemSubcategory, otherDetails, customerComments, firstName, lastName, email, phone1, address, city, xcoordinate, ycoordinate, resolvedAddress, locality, neighborhood, postal_code, route, street_number, image);
                return re;
            }
            catch (Exception e)
            {
                log.Error("In SetDetailsRest: " + e.Message);
            }
            return null;
        }


        [WebMethod]
        public TCBMP ValidateTCBMP(String tcbmpid, String inspId)
        {
            string[] inputs = { tcbmpid,  inspId};
            if (inputs.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                TCBMP tcresult = new TCBMP();
                var message = "Required Fields Missing. Fields required are tcbmpid,  inspId";
                tcresult.Message = message;
                log.Info(message);
                return tcresult;
            }

            var cosdEnv = WebConfigurationManager.AppSettings["cosdEnv"];
            String result = AccelaRestServiceSO.AuthenticateRestUser("COSD", cosdEnv);
            log.Info("In ValidateTCBMP parameter: " + tcbmpid+ ":" + inspId);
            var token = result;
            log.Info("In SetDetailsRest token: " + token);
            try
            {
                var re = AccelaRestServiceSO.ValidateTCBMP(token, tcbmpid, inspId);
                return re;
            }
            catch (Exception e)
            {
                log.Error("In SetDetailsRest: " + e.Message);
            }
            return null;
        }
        [WebMethod]
        public String TCBMPSetDocument(String token, String image, String inspId, String recordId)
        {
            string[] inputs = {token, image, inspId };
            if (inputs.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                var message = "Required Fields Missing. Fields required are  token, image, inspId, recordId";
                return message;
            }

            log.Info("In TCBMPSetDocument parameter: " + image + ":" + inspId);
            log.Info("In TCBMPSetDocument token: " + token);
            try
            {
                var re = AccelaRestServiceSO.TCBMPSetDocument(token, image, inspId, recordId);
                return re;
            }
            catch (Exception e)
            {
                log.Error("In SetDetailsRest: " + e.Message);
            }
            return null;
        }

        //[WebMethod]
        //public string AuthenticateRestUser(String Agency)
        //{
        //    log.Info("In AuthenticateRestUser parameter: " + Agency);
        //    try
        //    {
        //        var re = AccelaRestServiceSO.AuthenticateRestUser(Agency);

        //        if (!string.IsNullOrWhiteSpace(re))
        //        {
        //            log.Info("In AuthenticateRestUser return: " + "OK");

        //        }
        //        else
        //        {
        //            log.Error("In AuthenticateRestUser return: " + "NO TOKEN RETURNED");
        //        }
        //        return re;
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("In AuthenticateRestUser: " + e.Message);
        //    }
        //    return "";
        //}

        //[WebMethod(EnableSession = true)]
        //public string SetDocumentAsync(String recordid, String image)
        //{
        //    var token = (string)(Session["token"]);
        //    try
        //    {
        //        var ci = AccelaRestServiceSO.SetDocumentAsync(token, recordid, image);
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.Message);
        //    }
        //    return null;
        //}

        //[WebMethod]
        //public string AuthenticateUser(String Agency)
        //{
        //    try
        //    {
        //        var usr = WebConfigurationManager.AppSettings["usr"];
        //        var passwd = WebConfigurationManager.AppSettings["passwd"];
        //        return AccelaRestServiceSO.AuthenticateUser(Agency, usr, passwd);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.Message);
        //    }
        //    return "";
        //}

        //[WebMethod(EnableSession = true)]
        //public string SetDetails(String problemCategory, String problemSubcategory, 
        //    String otherDetails, String customerComments, String firstName, String lastName, String email, String phone1, String address, String city, 
        //    String xcoordinate, String ycoordinate, String resolvedAddress, String locality, String neighborhood, String postal_code, String route, String street_number, String image)
        //{
        //    //var token = "93575416890089405040992727108065039037975990860222323044995073785000760699435340431481429366038566509262602549358458803337589710";
        //    var token = (string)(Session["token"]);
        //    try
        //    {
        //        var ci = AccelaRestServiceSO.SetDetails(token, problemCategory, problemSubcategory, otherDetails, customerComments, firstName, lastName, email, phone1, address, city, xcoordinate, ycoordinate, resolvedAddress, locality, neighborhood, postal_code, route, street_number, image);
        //        return ci;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.Message);
        //    }
        //    return null;
        //}

        //[WebMethod(EnableSession = true)]
        //public List<Addr> ValidateAddress(String street_number, String route, String neighborhood, String locality, String postal_code)
        //{
        //    //var token = "93575416890089405040992727108065039037975990860222323044995073785000760699435340431481429366038566509262602549358458803337589710";
        //    var token = (string)(Session["token"]);
        //    try
        //    {
        //        var ci = AccelaRestServiceSO.SearchAddresses(token, street_number, route, neighborhood, locality, postal_code);
        //        return ci;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.Message);
        //    }
        //    return null;
        //}

        //[WebMethod(EnableSession = true)]
        //public TCBMP ValidateTCBMP(String tcbmpid)
        //{
        //    //var token = "75473855968888427117597405910475910809426019813378647086111300608928799630592681282401306386646215915503653975744583877872185249";
        //    var token = (string)(Session["token"]);
        //    try
        //    {
        //        var ci = AccelaRestServiceSO.ValidateTCBMP(token, tcbmpid);
        //        return ci;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.Message);
        //    }
        //    return null;
        //}

        //[WebMethod(EnableSession = true)]
        //public string SetTCBMPData(String tcbmpid, String image)
        //{
        //    //var token = "75473855968888427117597405910475910809426019813378647086111300608928799630592681282401306386646215915503653975744583877872185249";
        //    var token = (string)(Session["token"]);
        //    try
        //    {
        //        var ci = AccelaRestServiceSO.SetTCBMPData(token, tcbmpid, image);
        //        return ci;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.Message);
        //    }
        //    return null;
        //}

    }
}
