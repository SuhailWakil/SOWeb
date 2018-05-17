using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SOWebService.Accela;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;
using System.Web.Configuration;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization.Json;
using RestSharp;
using Newtonsoft.Json.Linq;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace SOWebService.Accela
{
    public class AccelaRestServiceSO
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string AuthenticateRestUser(string agency, string env)
        {            
            var authServer = WebConfigurationManager.AppSettings["constServer"] + "/oauth2/token";
            var client_id = WebConfigurationManager.AppSettings["client_id"];
            var client_secret = WebConfigurationManager.AppSettings["client_secret"];
            var usr = WebConfigurationManager.AppSettings["usr"];
            var passwd = WebConfigurationManager.AppSettings["passwd"];
            var environment = env;
            var scope = WebConfigurationManager.AppSettings["scope"];

            var client = new RestClient(authServer);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", 
                "grant_type=password&client_id="+ client_id + "&client_secret="+ client_secret + 
                "&username="+ usr + "&password="+ passwd + "&scope="+ scope + "&agency_name=" + agency +
                "&environment="+ environment, ParameterType.RequestBody);
            
            IRestResponse response = client.Execute(request);
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Message message = serializer.Deserialize<Message>(response.Content);
            log.Info("In AuthenticateRestUser response: access_token= " + "OK");

            if (response.StatusDescription.Equals("OK"))
            {
                var token = message.access_token;
                return token;
            }
            return "";
        }

        public static SOResult SetDetailsRest(string token, string problemCategory, string problemSubcategory, string otherDetails, string customerComments, string firstName,
            string lastName, string email, string phone1, string address, string city, string xcoordinate, string ycoordinate, string resolvedAddress,
            string locality, string neighborhood, string postal_code, string route, string street_number, string image)
        {
            var scriptServer = WebConfigurationManager.AppSettings["constServer"] + "/v4/scripts/SO_SET_RECORD";

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            var serializedResult = serializer.Serialize(new Dictionary<string, string>
            {
                ["problemCategory"] = problemCategory,
                ["problemSubcategory"] = problemSubcategory,
                ["otherDetails"] = otherDetails,
                ["customerComments"] = customerComments,
                ["firstName"] = firstName,
                ["lastName"] = lastName,
                ["email"] = email,
                ["phone1"] = phone1,
                ["address"] = !string.IsNullOrWhiteSpace(address) ? address : string.Empty,
                ["city"] = !string.IsNullOrWhiteSpace(city) ? city : string.Empty,
                ["xcoordinate"] = xcoordinate,
                ["ycoordinate"] = ycoordinate,
                ["resolvedAddress"] = resolvedAddress,
                //["refaddressid"] = refaddressid,
                ["locality"] = locality,
                ["neighborhood"] = neighborhood,
                ["postal_code"] = postal_code,
                ["route"] = route,
                ["street_number"] = street_number,
                ["base64String"] = image
            });

            var client = new RestClient(scriptServer);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", serializedResult, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            Message message = serializer.Deserialize<Message>(response.Content);
            log.Info("In SetDetailsRest response Message: " + message.result.Message + " status: " + message.status + " code: "+message.code + " err_message: "+ message.message);

            if (message.status == 200)
            {
                string soResultData = message.result.Message;
                SOResult soResult = getSOResult(soResultData);
                //soResult.token = token;
                soResult.Message = "OK";
                return soResult;
            }
            return null;
        }

         public static SOResult getSOResult(string output)
        {
            SOResult soresult = new SOResult();
            var ser = new DataContractJsonSerializer(typeof(SOResult));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(output)))
            {
                soresult = (SOResult)ser.ReadObject(ms);
            }
            return soresult;
        }

        public static TCBMP ValidateTCBMP(string token, string tcbmpid, string inspId)
        {
            var scriptServer = WebConfigurationManager.AppSettings["constServer"] + "/v4/scripts/SO_SEARCH_TCBMP";

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            var serializedResult = serializer.Serialize(new Dictionary<string, string>
            {
                ["recordID"] = tcbmpid,
                ["inspId"] = inspId
            });

            var client = new RestClient(scriptServer);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", serializedResult, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            Message message = serializer.Deserialize<Message>(response.Content);
            log.Info("In SetDetailsRest response Message: " + message.result.Message + " status: " + message.status + " code: " + message.code + " err_message: " + message.message);

            if (message.status == 200)
            {
                string tcbmpData = message.result.Message;
                TCBMP tcbmpdata =  getTCBMP(tcbmpData);
                tcbmpdata.token = token;
                return tcbmpdata;
            }
            return null;
        }
        public static TCBMP getTCBMP(string output)
        {
            TCBMP tcbmp = new TCBMP();
            var ser = new DataContractJsonSerializer(typeof(TCBMP));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(output)))
            {
                tcbmp = (TCBMP)ser.ReadObject(ms);
            }
            return tcbmp;
        }

        public static String TCBMPSetDocument(string token, string image, string inspId, string recordId)
        {
            var scriptServer = WebConfigurationManager.AppSettings["constServer"] + "/v4/scripts/SVD_SET_INSPECTION_DOC";

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            var serializedResult = serializer.Serialize(new Dictionary<string, string>
            {
                ["base64String"] = image,
                ["inspId"] = inspId,
                ["recordId"] = recordId
            });

            var client = new RestClient(scriptServer);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", serializedResult, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            Message message = serializer.Deserialize<Message>(response.Content);
            log.Info("In TCBMPSetDocument response Message: " + message.result.Message + " status: " + message.status + " code: " + message.code + " err_message: " + message.message);

            if (message.status == 200)
            {
                string soResultData = message.result.Message;
                return soResultData;
            }
            return null;
        }

        //public static String UploadInspDoc() {
        //    var documentBytes = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAACF0lEQVR4nJWRv2tTURTHPzc8GkkJbV/FlBBL/YGCVhBHzSBR0KG+qU5ubiGTuDqJq3P/ArHQghASqIMhCs3iUkjsUFv6UiUImpeESEhy373X4aVJ04jggcu93O853/M95ysAbNteBdJCiBSDMMYUgDXP8zb5RwiA2dnZD47jpJaWloaA67rk8/m/FimlCsaYtVartSkA5ubmTDKZpNVqDZOmrzYIL+8DEPt9l7OdW0OsWq2Sy+UKzWbznrBte1UIsXG6y4UndV48vwLAq9d7HL6Zn1BijHlsaa3TjuNwUj7AR/slzcheoOYiZDKZMdx1XbLZbNoSQqQajQau646zP4JKPXhrCcVicQyfmZlBCJGyjDF4nje5qT5M6QFBH1BqDPY8D2MMljEGdQxO9eDSPmKxDX04HzlBcH8bcxSFg8vQDx/vAEtrjZQSgNDiEXecNg9uL9PuWnxu7gDw7OlNomd83pcqbL89Qh8E+9JaBwS+7wMg3Bif1rv8alewFsBrBwp+Rnfwf8BufhrtxjCDfK31qRE6IdhLUJbfuLbSYX4h+O5+h91cBHWYABkC1PgIxwoA8AXqa5zyuxrXVzoAfMlFUG4cIwUwyh0qsCyLXq83IlECdXCO8kbgjl+zB8UjJ8LhcKBASrmllHoYi8UmrewmgtuehOr1OlLKLQAHKAHmP08JcMSAMArcAOKANdlvLHygBpSB9h9CLA5gdXz3AwAAAABJRU5ErkJggg==";

        //    byte[] bytes = Convert.FromBase64String(documentBytes);
        //    Image image;
        //    using (MemoryStream ms = new MemoryStream(bytes))
        //    {
        //        image = Image.FromStream(ms);
        //    }
        //    //string decodedString = Encoding.UTF8.GetString(data);

        //    var client = new RestClient("https://apis.accela.com/v4/inspections/5696082/documents");
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("postman-token", "b5b1c3bd-ed0c-8f2d-7555-40588f7898b8");
        //    request.AddHeader("cache-control", "no-cache");
        //    request.AddHeader("authorization", "v197mCRiztjF-gw6wZ-C46KN5c8E2n3HRcrq1e5pb4L5rNiC3vtWT003ur2UCTC9o1JtoyAyTXh_2WqQQdwLV1BGw-LpSl6EiDNanJiOALrRAINkBbboXWe05chSPmbuR2NIvJWHCbBRO-D_Iq7tdg0JRq0PJ-jtK3cqGyhjlMcqegTA-sGxJg2HtB05FGOXPkjuW1y5n_tFeggUDfU3tL1kP3jY94QxbHX6PcT-4QnEGNSdZbVUPzpymso5ewBhDADOLFfi2SrZpkHS5B8PF3Cbfb65ODgndEN2EDUwGdEaA5w6f7_jlXOx3w-gTV2f6mfyJiXc4Acqs_rG17gjaa8iqCa13nup7ISQJhTwjScpXHIG4X4BlxDvK8s8AYq0S4j9g45SA5tQYvsZnAPDfYztyvTFc0CB0h5CHCRqiAyeE_uQ-OtBRXRNG2ClRvXSUX1MJm7qz-VzF3tlqNxMcY_OOB5fkZEkxhQQWF-8BBJIekxwqQqlR7k10chmWRvplLhfJSoeypKcaRzhe6QVSX3tqI0g1_ugxAYUJCsF0SY1");
        //    request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
        //    request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"fileInfo\"\r\n\r\n[\n   {\n      \"serviceProviderCode\": \"COSD\",\n      \"fileName\": \"sw1.gif\",\n      \"type\": \"image/gif\",\n      \"description\": \"sw1\"\n   }\n]\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"uploadedFile\"; filename=\"2.gif\"\r\nContent-Type: image/gif\r\n"+
        //        bytes + "\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
        //    IRestResponse response = client.Execute(request);


        //    return "";
        //}
        //public static string AuthenticateUser(string agency, string username, string password)
        //{
        //    log.Info("Hello logging world!");
        //    Console.WriteLine("Hit enter");
        //    //Console.ReadLine();

        //    var serializer = new JavaScriptSerializer();
        //    var serializedResult = serializer.Serialize(new Dictionary<string, string>
        //    {
        //        ["agency"] = agency,
        //        ["userId"] = username,
        //        ["password"] = password
        //    });

        //    var url = "/apis/agency/auth";

        //    dynamic output = SendRestRequest(url, "POST", serializedResult);
        //    return output.ContainsKey("result") ? output["result"] : null;
        //}

        //public static string SetDetails(string token, string problemCategory, string problemSubcategory, string otherDetails, string customerComments, string firstName, 
        //    string lastName, string email, string phone1, string address, string city, string xcoordinate, string ycoordinate, string resolvedAddress, 
        //    string locality, string neighborhood, string postal_code, string route, string street_number, String image)
        //{
        //    var serializer = new JavaScriptSerializer();
        //    var serializedResult = serializer.Serialize(new Dictionary<string, string>
        //    {
        //        ["problemCategory"] = problemCategory,
        //        ["problemSubcategory"] = problemSubcategory,
        //        ["otherDetails"] = otherDetails,
        //        ["customerComments"] = customerComments,
        //        ["firstName"] = firstName,
        //        ["lastName"] = lastName,
        //        ["email"] = email,
        //        ["phone1"] = phone1,
        //        ["address"] = !string.IsNullOrWhiteSpace(address) ? address : string.Empty,
        //        ["city"] = !string.IsNullOrWhiteSpace(city) ? city : string.Empty,
        //        ["xcoordinate"] = xcoordinate,
        //        ["ycoordinate"] = ycoordinate,
        //        ["resolvedAddress"] = resolvedAddress,
        //        //["refaddressid"] = refaddressid,
        //        ["locality"] = locality,
        //        ["neighborhood"] = neighborhood,
        //        ["postal_code"] = postal_code,
        //        ["route"] = route,
        //        ["street_number"] = street_number,
        //        ["base64String"] = image
        //    });

        //    var url = $"/apis/v4/scripts/SO_SET_RECORD?token={token}";
        //    dynamic output = SendRestRequest(url, "POST", serializedResult);

        //    if (output.ContainsKey("result"))
        //    {
        //        string recordid = output["result"]["Message"].Trim('"');
        //        //SetDocumentAsync(token, recordid, image);
        //        return recordid;
        //    }
        //    return null;
        //}

        //public static string SetTCBMPData(string token, string tcbmpid, String image)
        //{
        //    SetDocumentAsync(token, tcbmpid, image);
        //    return tcbmpid;
        //}

        //public static List<Addr> SearchAddresses(string token, string street_number, string route, string neighborhood, string locality, string postal_code)
        //{
        //    var serializer = new JavaScriptSerializer();
        //    var serializedResult = serializer.Serialize(new Dictionary<string, string>
        //    {
        //        ["street_number"] = street_number,
        //        ["route"] = route,
        //        ["neighborhood"] = neighborhood,
        //        ["locality"] = locality,
        //        ["postal_code"] = postal_code
        //    });

        //    var url = $"/apis/v4/scripts/SO_SEARCH_ADDRESSES?token={token}";
        //    dynamic output = SendRestRequest(url, "POST", serializedResult);
        //    if (output.ContainsKey("result"))
        //    {
        //        return getAddList(output["result"]["Message"]);
        //    }
        //    return null;
        //}
        //public static List<Addr> getAddList(string output)
        //{
        //    List<Addr> addList = new List<Addr>();
        //    var ser = new DataContractJsonSerializer(typeof(List<Addr>));
        //    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(output)))
        //    {
        //        addList = (List<Addr>)ser.ReadObject(ms);
        //    }

        //    return addList;
        //}
        //public static TCBMP ValidateTCBMP(string token, string tcbmpid)
        //{
        //    var serializer = new JavaScriptSerializer();
        //    var serializedResult = serializer.Serialize(new Dictionary<string, string>
        //    {
        //        ["recordID"] = tcbmpid
        //    });

        //    var url = $"/apis/v4/scripts/SO_SEARCH_TCBMP?token={token}";
        //    dynamic output = SendRestRequest(url, "POST", serializedResult);

        //    if (output.ContainsKey("result"))
        //    {
        //        return getTCBMP(output["result"]["Message"]);
        //    }
        //    return null;
        //}
        //public static TCBMP getTCBMP(string output)
        //{
        //    TCBMP tcbmp = new TCBMP();
        //    var ser = new DataContractJsonSerializer(typeof(TCBMP));
        //    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(output)))
        //    {
        //        tcbmp = (TCBMP)ser.ReadObject(ms);
        //    }

        //    return tcbmp;
        //}

        //public static Task SetDocumentAsync(string token, String recordid, String image)
        //{
        //    var result = Task.Factory.StartNew(() => SetDocument(token, recordid, image));
        //    return result;
        //}

        //public static async Task SetDocument(string token, String recordid, String image)
        //{
        //    String base64String = image;
        //    var serializer = new JavaScriptSerializer();
        //    var serializedResult = serializer.Serialize(new Dictionary<string, string>
        //    {
        //        ["recordid"] = recordid,
        //        ["base64String"] = !string.IsNullOrWhiteSpace(base64String) ? base64String : string.Empty
        //    });

        //    var client = new RestClient("https://apis.accela.com/v4/scripts/SO_SET_RECORD_DOCS");
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("cache-control", "no-cache");
        //    request.AddHeader("authorization", "ID7_8T0CdSW4H-HrZRPVNGKyvQurnnOINLXl8_sWP4ErGRz8Z5o9D08NAO1iZb_er_npEUWKRtbeWL33Ls1XmJ0DvLLRJQJFD5ZiHBJJOLN48E-irjQkvvNZA2bz-sw5tmlh7ArcEIU872neqehUtac7u4XKKLU9BTJ6JKwVmCPCjldQ5yhDy0V0IPbeOW1cKO6UK8WyPWpv-9poAeOxAG0fERzlpo5VRnTNqRX0oZJ_Jm2jnemDtdIv1dbwPTZah3Gk2wxX8UBsJLtz9Vsw9ki26sif4tLrkMxD7MpxgP869Lez2jwGxDzsYIY43neoSvVy1bHktnSdviKz1D9VGMZvClWj-R7RQNbT1z-NNBH06V7PAEZ0fzQaWTXmQbET0Ac1tlaodpPzqEiGmqDO3UpCgoeR-LiKJtpNuoDsHkbuWnOsDuSN8JBqaYy7QnHkhiEKb-dOp8k9QDTZluq5rqfbijtP1_scn9bPfAWO1eI1");
        //    request.AddHeader("content-type", "application/json");
        //    request.AddParameter("application/json", serializedResult, ParameterType.RequestBody);
        //    await Task.Delay(5000);
        //    IRestResponse response = client.Execute(request);

        //    //var url = $"/apis/v4/scripts/SO_SET_RECORD_DOCS?token={token}";
        //    //SendRestRequest(url, "POST", serializedResult);
        //}


        //REQUEST HELPERS
        //public static dynamic SendRestRequest(string url, string method, string body)
        //{
        //    var data = new byte[0];
        //    if (method.ToUpper().Equals("POST") || method.ToUpper().Equals("PUT"))
        //    {
        //        data = Encoding.UTF8.GetBytes(body);
        //    }

        //    return SendRestRequest(url, method, data);
        //}

        //public static dynamic SendRestRequest(string url, string method, byte[] data = null)
        //{
        //    var bizServer = WebConfigurationManager.AppSettings["BizServer"];
        //    url = bizServer + url;

        //    var responseString = SendWebRequest(url, method, data);

        //    var serializer = new JavaScriptSerializer();

        //    dynamic output;
        //    try
        //    {
        //        // in case we fail to parse the json return an error
        //        output = serializer.Deserialize<object>(responseString);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("", new Exception($"{method} request to {url} returned invalid JSON. Exception: {e.Message}"));
        //    }

        //    if (output.ContainsKey("status"))
        //    {
        //        var status = output["status"];

        //        if (status != 200)
        //        {
        //            var errorMessage =
        //                $"An error occured while trying to send a {method} request to {url}. Status: {status}, error code: {(output.ContainsKey("code") ? output["code"] : "Not Specified")}, message: {(output.ContainsKey("message") ? output["message"] : "Not Specified")}";
        //            throw new Exception(output.ContainsKey("message") ? output["message"] : "", new Exception(errorMessage));
        //        }
        //    }

        //    // If the server returned a proper JSON but without a status we'll consider the request a success
        //    return output;
        //}

        //public static string SendWebRequest(string url, string method, byte[] data = null, bool useProxy = false)
        //{
        //    if (string.IsNullOrWhiteSpace(url))
        //    {
        //        throw new ArgumentException("Request URL is required");
        //    }

        //    if (string.IsNullOrWhiteSpace(method))
        //    {
        //        throw new ArgumentException("Request Method is required");
        //    }

        //    data = data ?? new byte[0];

        //    if (method.ToUpper().Equals("POST") || method.ToUpper().Equals("PUT"))
        //    {
        //        if (data.Length == 0)
        //        {
        //            throw new ArgumentException("Request Body is required");
        //        }
        //    }

        //    string responseString;
        //    try
        //    {

        //        var requestUri = new Uri(url);
        //        var request = (HttpWebRequest)WebRequest.Create(requestUri);
        //        request.Method = method;
        //        request.ContentType = "application/json";
        //        request.AllowAutoRedirect = false;
        //        request.Timeout = 300000;
        //        //request.Timeout = 500;
        //        if (useProxy)
        //        {
        //            var proxy = WebConfigurationManager.AppSettings["proxy"];

        //            if (proxy.Length > 0)
        //            {
        //                var webproxy = new WebProxy(proxy);
        //                request.Proxy = webproxy;
        //            }
        //        }

        //        if (method.ToUpper().Equals("POST") || method.ToUpper().Equals("PUT"))
        //        {
        //            request.ContentLength = data.Length;

        //            using (var stream = request.GetRequestStream())
        //            {
        //                stream.Write(data, 0, data.Length);
        //            }
        //        }

        //        var response = (HttpWebResponse)GetResponseWithoutException(request);

        //        using (var stream = response.GetResponseStream())
        //        {
        //            if (stream == null)
        //            {
        //                throw new Exception("", new Exception($"{method} request to {url} didn't return a response"));
        //            }
        //            var reader = new StreamReader(stream, Encoding.UTF8);
        //            responseString = reader.ReadToEnd();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message,
        //            new Exception($"An exception occured while trying to send a {method} request to {url}. Exception: {e.Message}"));
        //    }

        //    if (string.IsNullOrEmpty(responseString))
        //    {
        //        throw new Exception("", new Exception($"{method} request to {url} didn't return a response"));
        //    }

        //    return responseString;
        //}

        //public static WebResponse GetResponseWithoutException(WebRequest request)
        //{
        //    try
        //    {
        //        return request.GetResponse();
        //    }
        //    catch (WebException e)
        //    {
        //        if (e.Response == null)
        //        {
        //            throw;
        //        }
        //        return e.Response;
        //    }
        //}
    }
}




