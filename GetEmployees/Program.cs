using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
//using System.Web.Extensions;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace GetEmployees
{
    class Program
    {
        static string serializedEst;
        static DateTime dlrTime;
        static DateTime utcDateStart;
        static DateTime utcDateEnd;
        static string deltaDateStart;
        static string deltaDateEnd;
        static int Identity = 0;
        static string iHrs;
        static string iMins;
        static string iDay;
        public static string authkey = "";
        public static int DmsId = 0;
        public static int EndUserId = 0;
        public static string EndUserTitle = "";
        public static string ServiceURL = "";
        public static string APIKey = "";
        public static string SecretKey = "";
        public static string SubscriptionId = "";
        public static int SrcType =0;
        static void Main(string[] args)
        {
            string positionName = "";
            int PositionId = 0;
            GetVSToken();
            string time_zone = String.Empty;
            SrcType = 1; // it's an position srctype value
            List<CdkDealers> dlrList = BL_eLead.GetDMSActiveDealers(1, SrcType, ""); // 1 refers DMS Type Source Name is "CDK"
            char IsCompanyPositionsCallDone = 'N';
            foreach (CdkDealers row in dlrList)
            {
                DmsId = row.DMS_ID;
                EndUserId = row.DMS_ENDUSER_ID;
                EndUserTitle = row.DMS_ENDUSER_TITLE;
                ServiceURL = row.DSTDP_SERVICE_URL;
                APIKey = row.DS_APIKEY;
                SecretKey = row.DS_SECRETKEY;
                SubscriptionId = row.DED_SUBSCRIPTIONID;

                //time_zone = row.ENDUSER_TIMEZONE;
                //int zSpanHr = Convert.ToInt32(row.ZONESPAN.Split('.')[0]);
                //int zSpanMin = Convert.ToInt32(row.ZONESPAN.Split('.')[1]);

                //TimeZoneInfo earlyEstZone = TimeZoneInfo.CreateCustomTimeZone(time_zone,
                //                           new TimeSpan(zSpanHr, zSpanMin, 0),
                //                           time_zone,
                //                           time_zone);

                //serializedEst = earlyEstZone.ToSerializedString();
                //TimeZoneInfo cstZone = TimeZoneInfo.FromSerializedString(serializedEst);
                //dlrTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cstZone);
                dlrTime = row.ZONETIME; 
                int dlrTm = dlrTime.Hour;
                int dlrMin = dlrTime.Minute;
                int dlrDay = dlrTime.Day;
                iDay = dlrDay.ToString();



                iHrs = dlrTm.ToString();
                if (Convert.ToInt32(iHrs) <= 9)
                    iHrs = "0" + iHrs;

                iMins = dlrMin.ToString();
                if (Convert.ToInt32(iMins) <= 9)
                    iMins = "0" + iMins;

                if (dlrTm == 5)
                { GetCompanyPositions(dlrTm); IsCompanyPositionsCallDone = 'Y'; }
            }



            /*
             getting employees by position
             */
            SrcType = 2; // (2): it's an Employee srctype value
            dlrList = BL_eLead.GetDMSActiveDealers(1, SrcType, ""); // 1 refers DMS Type Source Name is "CDK"
            List<PositionsDeltaAction> Positions = BL_eLead.PositionsDeltaAction("D","",59);
           
            if (IsCompanyPositionsCallDone == 'Y')
            {
                foreach (CdkDealers row in dlrList)
                {
                    DmsId = row.DMS_ID;
                    SubscriptionId = row.DED_SUBSCRIPTIONID;
                    List<CompanyPositions> positionList = BL_eLead.GetCompanyPositions(DmsId); // 1 refers DMS Type Source Name is "CDK"

                    JObject JPositions = new JObject();
                    JArray JArrPositions = new JArray();
                    JObject JPosition = new JObject();
                    JArray JArrPosition = new JArray();
                    foreach (CompanyPositions prow in positionList)
                    {
                        JObject jPostion = new JObject();
                        
                        jPostion.Add("PositionId", prow.CPOS_ID);
                        jPostion.Add("positionName", prow.CPOS_NAME);
                        PositionId = prow.CPOS_ID;
                        positionName = prow.CPOS_NAME;

                       string employees =  GetEmployeesByPosition(prow.CPOS_LINKS_HREF, prow.CPOS_LINKS_METHOD);
                        if (employees != "error")
                        {
                            if (employees != "")
                            {
                                var jObject1 = JObject.Parse(employees);
                                jPostion.Merge(jObject1);
                                JArrPosition.Add(new JObject(jPostion));
                            }
                        }
                    }
                    JPosition["employees"] = JArrPosition;
                    //JArrPositions.Add(JPosition);
                    //JPositions["positions"] = JArrPositions;

                    XmlDocument empdoc = JsonConvert.DeserializeXmlNode(JPosition.ToString(), "positions");
                    var empstringWriter = new StringWriter();
                    var empxmlTextWriter = XmlWriter.Create(empstringWriter);
                    empdoc.WriteTo(empxmlTextWriter);
                    empxmlTextWriter.Flush();
                    string str = empstringWriter.ToString();
                    str = empdoc.OuterXml.ToString();
                    int XmlRecCount = 0;
                    try
                    {
                        //XmlNodeList xmlNL = doc.GetElementsByTagName("items");
                        //XmlRecCount = xmlNL.Count; C:\eLeads\Employees
                        string filePath = "D:\\DMS\\eLeads\\companyEmployees\\" + DmsId  + ".xml";
                        empdoc.Save(filePath);
                        DA_eLead.LogInsertion(DmsId, str, "0", "", "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    //    string CustomerbyOppDeltaAction = BL_Opportunities.CustomerbyOpportunityDeltaAction(SrcType, DmsId, LogId, filePath, "D");
                    }
                    catch (Exception e)
                    {
                        var w32ex = e as Win32Exception;
                        string ErrorCode = "";
                        if (w32ex == null)
                        {
                            w32ex = e.InnerException as Win32Exception;
                            ErrorCode = Convert.ToString(w32ex.ErrorCode);
                        }
                        if (w32ex != null)
                        {
                            ErrorCode = Convert.ToString(w32ex.ErrorCode);
                            // do stuff
                        }
                        DA_eLead.LogInsertion(DmsId, str, ErrorCode, e.Message, "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    }
                }
            }
            else
            {
                Console.WriteLine("Still Dealer time is "+ dlrTime.ToString());
            }
            List<PositionsDeltaAction> Employees = BL_eLead.EmployeesDeltaAction("D", "", 59);
        }

        public static void GetVSToken()
        {
            try { 
            string[] TokenExpire = new string[2];
            TokenExpire = DA_eLead.CheckVSTokenExpiry();
            if (TokenExpire[0] == "0")
                authkey = GetBearerToken();
            else
                authkey = TokenExpire[1];
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static string GetBearerToken()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string resultData = "";
            WebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                string url = "https://identity.fortellis.io/oauth2/aus1p1ixy7YL8cMq02p7/v1/token";
                string postData = "grant_type=client_credentials&client_secret=fakAFPIArhO2pp3p&scope=anonymous&client_id=heInundwhF8iHEByMAHwBFxkGGAc1rPk";
                //string postData = "grant_type=client_credentials&client_secret="+SecretKey+"&scope=anonymous&client_id="+APIKey+"";
                //encode post data     
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(postData);
                request = HttpWebRequest.Create(url);
                request.Method = "POST";
                request.ContentLength = data.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                CredentialCache mycache = new CredentialCache();
                request.Credentials = mycache;
                Stream newStream = request.GetRequestStream();
                // Send the post data.     
                newStream.Write(data, 0, data.Length);
                newStream.Close();
                response = request.GetResponse();
                stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                resultData = sr.ReadToEnd();
                stream.Close();
                stream.Dispose();
                Item Item = JsonConvert.DeserializeObject<Item>(resultData);
                authkey = Item.access_token.ToString();
                int err_no = DA_eLead.InsertVSToken(Item.access_token, Item.expires_in, Item.token_type);
            }
            catch (Exception e)
            {
            }
            return authkey;
        }

        public static void GetCompanyPositions(int dlrTm)
        {
            string resultData = "";
            WebRequest request = null;
            WebResponse response = null;

            restartPosition:
            try
            {
                //string url = "https://api.fortellis.io/sales/v1/elead/productreferencedata/companyPositions";ServiceURL+"/companyPositions";
                string url = ServiceURL;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ASCIIEncoding encoding = new ASCIIEncoding();
                request = HttpWebRequest.Create(url);
                request.Method = "GET";
                //request.ContentType = "application/json";
                CredentialCache mycache = new CredentialCache();
                mycache.Add(new Uri(url), "No", new NetworkCredential());
                request.Credentials = mycache;
                request.Headers.Add("Subscription-Id", SubscriptionId);
                request.Headers.Add("Request-Id", "owned-vehicles");
                request.Headers.Add("api_key", APIKey);
                request.Headers.Add("Authorization", "Bearer " + authkey);
                //byte[] bytes1 = Encoding.UTF8.GetBytes("");
                //request.ContentLength = Convert.ToInt32(bytes1.Length);
                //using (Stream stream = request.GetRequestStream())
                //{
                //    stream.Write(bytes1, 0, bytes1.Length);
                //    stream.Close();
                //}
                response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                // Do whatever you need with the response
                Byte[] myData = ReadFully(responseStream);
                resultData = System.Text.ASCIIEncoding.ASCII.GetString(myData);
                XmlDocument doc = JsonConvert.DeserializeXmlNode(resultData, "companyPositions");
                var stringWriter = new StringWriter();
                var xmlTextWriter = XmlWriter.Create(stringWriter);
                doc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                responseStream.Close();
                string str = stringWriter.ToString();
                str = doc.OuterXml.ToString();
                int XmlRecCount = 0;
                try
                {
                    XmlNodeList xmlNL = doc.GetElementsByTagName("items");
                    XmlRecCount = xmlNL.Count;
                    string filePath = "D:\\DMS\\eLeads\\companyPositions\\" + DmsId + ".xml";
                    doc.Save(filePath);
                    DA_eLead.LogInsertion(DmsId, str, "0", "", "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                  

                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        // protocol errors find the statuscode in the Response
                        // the enum statuscode can be cast to an int.
                        int code = (int)((HttpWebResponse)e.Response).StatusCode;
                        string content;
                        using (var reader = new StreamReader(e.Response.GetResponseStream()))
                        {
                            content = reader.ReadToEnd();
                        }
                        // do what ever you want to store and return to your callers
                        dynamic errObj = JObject.Parse(content);
                        string errMsg = errObj.code + " - " + errObj.message;
                        if (code == 401 && errObj.message == "Invalid Bearer Token - Token Expired")
                        {
                            authkey = GetBearerToken();
                            goto restartPosition;
                        }
                        else
                        {
                            DA_eLead.ErrorLogInsertion(DmsId, code.ToString(), "Error1 :" + errMsg, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                            DA_eLead.LogInsertion(DmsId, "", code.ToString(), "Error1 :"+errMsg, "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                        }
                    }
                    else
                    {
                         DA_eLead.ErrorLogInsertion(DmsId, e.HResult.ToString(), "Error2 :" + e.Message, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                         DA_eLead.LogInsertion(DmsId, "", e.HResult.ToString(), "Error2 :" + e.Message, "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    }

                }
                catch (Exception e)
                {
                    var w32ex = e as Win32Exception;
                    string ErrorCode = "";
                    if (w32ex == null)
                    {
                        w32ex = e.InnerException as Win32Exception;
                    }
                    if (w32ex != null)
                    {
                        ErrorCode = Convert.ToString(w32ex.ErrorCode);
                        // do stuff
                    }
                    DA_eLead.ErrorLogInsertion(DmsId, e.HResult.ToString(), "Error3 :" + e.Message, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    DA_eLead.LogInsertion(DmsId, str, ErrorCode, "Error3 :" + e.Message, "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                }
                //}

              
            }
            catch (Exception e)
            {
                //if (e.Message == "The remote server returned an error: (401) Unauthorized.")
                //{
                //    authkey = GetBearerToken();
                //    DA_eLead.LogInsertion(DmsId, "0", "401", e.Message, "D", "", "N", dlrTime, Convert.ToString(dlrTime), 1, 0);
                
                //}
            }
        }


        public static string GetEmployeesByPosition(string CPOS_LINKS_HREF, string CPOS_LINKS_METHOD)
        {
            restartEmployee:
            string resultData = "";
            WebRequest request = null;
            WebResponse response = null;
            try
            {
               // string url = "https://api.fortellis.io/sales/v1/elead/productreferencedata/companyEmployees?positionName=" + positionName;
                string url = CPOS_LINKS_HREF;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ASCIIEncoding encoding = new ASCIIEncoding();
                request = HttpWebRequest.Create(url);
                request.Method = CPOS_LINKS_METHOD;
                //request.ContentType = "application/json";
                CredentialCache mycache = new CredentialCache();
                mycache.Add(new Uri(url), "No", new NetworkCredential());
                request.Credentials = mycache;
                request.Headers.Add("Subscription-Id", SubscriptionId);
                request.Headers.Add("Request-Id", "owned-vehicles");
                request.Headers.Add("api_key", APIKey);
                request.Headers.Add("Authorization", "Bearer " + authkey);
                response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                // Do whatever you need with the response
                Byte[] myData = ReadFully(responseStream);
                resultData = System.Text.ASCIIEncoding.ASCII.GetString(myData);
                return resultData;
                //XmlDocument doc = JsonConvert.DeserializeXmlNode(resultData, "employee");
                //XmlNode root = doc.DocumentElement;
              
                //root.AppendChild(elem);
                //elem = doc.CreateElement("PositionId");
                //elem.InnerText = Convert.ToString(PositionId);
                //root.AppendChild(elem);
                
                //}
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    // protocol errors find the statuscode in the Response
                    // the enum statuscode can be cast to an int.
                    string code = ((HttpWebResponse)e.Response).StatusCode.ToString();
                    string content;
                    using (var reader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }
                    // do what ever you want to store and return to your callers
                    dynamic errObj = JObject.Parse(content);
                    string errMsg = errObj.code + " - " + errObj.message;
                    if (code == "401" && errObj.message == "Invalid Bearer Token - Token Expired")
                    {
                        authkey = GetBearerToken();
                        goto restartEmployee;
                    }
                    else if(code == "NotFound" && errObj.code=="CompanyEmployeesNotFound")
                    {
                        return "";
                    }
                    else
                    {
                        DA_eLead.ErrorLogInsertion(DmsId, code.ToString(), "Error1 :"+ errMsg, "D", "",  dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    }
                }
                else
                {
                     DA_eLead.ErrorLogInsertion(DmsId, e.HResult.ToString(), "Error2 :" + e.Message, "D", "",  dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                }
                return "error";
            }
            catch (Exception e)
            {
                //if (e.Message == "The remote server returned an error: (401) Unauthorized.")
                //{
                //    authkey = GetBearerToken();
                //    DA_eLead.LogInsertion(DmsId, "0", "401", e.Message, "D", "", "N", dlrTime, Convert.ToString(dlrTime), 2, 0);
                //}
                DA_eLead.ErrorLogInsertion(DmsId, e.HResult.ToString(), "Error3 :" + e.Message, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                return "error";
            }
            
        }

     
        public static byte[] ReadFully(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }

    public class Item
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
    }
}
