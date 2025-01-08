using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using IAMHeimdall.MVVM.Model;
using System.Collections.Generic;
using System.Globalization;

namespace IAMHeimdall.Core
{
    public class JSONMethods
    {
        #region Delegates

        #endregion

        #region Properties

        #endregion

        #region Methods
        public JSONMethods() { }
        #endregion

        #region Functions
        // Pass Username, Password, URL, JSON, Method, Type to get Return JSON Object
        public async Task<string> ReturnBody(string username, string password, string url, string json, string method, string type)
        {
            try
            {
                string returnedJSON = "";
                await Task.Run(() => 
                {
                    var uri = new Uri(url);
                    string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

                    // Define the API Request Object
                    WebRequest requestObj = (HttpWebRequest)WebRequest.Create(uri);
                    {
                        requestObj.Method = method;
                        requestObj.ContentType = type;
                        requestObj.PreAuthenticate = true;
                        requestObj.Proxy = new WebProxy
                        {
                            Credentials = CredentialCache.DefaultCredentials
                        };
                        requestObj.ContentLength = json.Length;
                        requestObj.Headers.Add("authorization", "Basic " + encoded);
                    };


                    using (Stream stream = requestObj.GetRequestStream())
                    {
                        StreamWriter sw = new(stream);
                        sw.Write(json);
                        sw.Flush();
                        stream.Close();
                    }

                    HttpWebResponse responseObjGet = null;
                    responseObjGet = (HttpWebResponse)requestObj.GetResponse();
                    var statusCode = responseObjGet.StatusCode;

                    // Response Object Returns Status Code, as Well as Status of Request 
                    using (Stream stream = responseObjGet.GetResponseStream())
                    {
                        StreamReader sr = new(stream);
                        string resultTest = sr.ReadToEnd();
                        string status = "";

                        //resultTest = resultTest.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                        var parsedJson = JObject.Parse(resultTest);
                        var token = (string)parsedJson["status"];
                        status = token;
                        sr.Close();

                        returnedJSON = parsedJson.ToString();
                    }
                });

                return returnedJSON;

            }
            catch (WebException ex)
            {
                string response = "";
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    response = reader.ReadToEnd();
                }
                var parsedJson = JObject.Parse(response);
                string status = "";
                status = (string)parsedJson["status"];
                ExceptionOutput.Output(ex.ToString());
                return status;
            }
        }

        // JSON Resources Class List to DataTable
        public async Task<DataTable> JSONToTable(List<AccessNowRequestModel.Resources> resourcesList)
        {
            DataTable tempTable = AccessNowRequestModel.OperationsTable();

            try
            {
                await Task.Run(() => 
                {
                    foreach (AccessNowRequestModel.Resources recList in resourcesList)
                    {
                        DataRow dr = tempTable.NewRow();

                        dr["request_id"] = recList.requestid;
                        dr["status"] = recList.status;

                        dr["request_type"] = recList.requestDesc;

                        dr["asset"] = ""; if (recList.asset != null)
                        {
                            dr["asset"] = recList.asset.Short_Name;
                        }
                        else if (recList.requestData != null)
                        {
                            if (recList.requestData.access != null) { dr["asset"] = recList.requestData.access.Asset; }
                        }
                        else if (recList.account != null) { dr["asset"] = recList.account.Asset; }

                        dr["access"] = recList.entityPropType + " " + recList.relatedEntityName;
                        dr["sub_request_type"] = recList.requestType;

                        dr["account_name"] = ""; if (recList.user != null) { dr["account_name"] = recList.user.Company_AD_Account_Name; }
                        dr["requestor_name"] = ""; if (recList.requestor != null) { dr["requestor_name"] = recList.requestor.Company_AD_Account_Name; }
                        dr["requestor_employee_id"] = ""; if (recList.requestor.Employee_ID != null) { dr["requestor_employee_id"] = recList.requestor.Employee_ID; }

                        dr["ticket_id"] = ""; if (recList.ticketData != null) { dr["ticket_id"] = recList.ticketData.ticketId; }
                        if (recList.status == "fulfilled" || recList.status == "rejectedduringfultillment") { dr["ticket_status"] = "Closed Complete"; }
                        else { dr["ticket_status"] = "Open"; }
                        dr["mail_to"] = ""; if (recList.additionalData != null) { dr["mail_to"] = recList.additionalData.fulfillmentData.Mail_Sent_To; }
                        dr["mail_sent_on"] = ""; if (recList.additionalData != null) { dr["mail_to"] = recList.additionalData.fulfillmentData.Mail_Sent_On; }


                        DateTime CreatedDate = new();
                        dr["requested_on"] = recList.requestedOn;
                        if (recList.requestedOn != null)
                        { DateTime.TryParseExact(recList.requestedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out CreatedDate); }

                        DateTime MailSentOnDate = new();
                        string MailSentOnDateText = "";
                        dr["mail_sent_on_date"] = ""; if (recList.additionalData != null)
                        {
                            dr["mail_sent_on_date"] = recList.additionalData.fulfillmentData.Mail_Sent_On_Date;
                            DateTime.TryParseExact(recList.additionalData.fulfillmentData.Mail_Sent_On_Date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out MailSentOnDate);
                            MailSentOnDateText = MailSentOnDate.ToString();
                        }

                        DateTime ClosedOnDate = new();
                        dr["closed_on"] = ""; if (recList.closedOn != null)
                        {
                            dr["closed_on"] = recList.closedOn;
                            DateTime.TryParseExact(recList.closedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ClosedOnDate);
                        }


                        string SLAShortDate = "Incomplete";

                        if (recList.closedOn != "" && recList.closedOn != null)
                        {
                            if (MailSentOnDateText == "") { SLAShortDate = (ClosedOnDate.Date - CreatedDate.Date).Days.ToString(); }
                            else { SLAShortDate = (ClosedOnDate.Date - MailSentOnDate.Date).Days.ToString(); }
                        }

                        dr["SLADays"] = SLAShortDate;

                        dr["TicketCount"] = "1";
                        dr["Category"] = "Category Filter";
                        dr["Type"] = "Type Filter";

                        tempTable.Rows.Add(dr);
                    }

                });

                return tempTable;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return tempTable;
            }
        }

        // JSON to Operations Resources Class List
        public async Task<AccessNowRequestModel.RootObject> JSONToRequestList (string jsonContent)
        {
            AccessNowRequestModel.RootObject rootObject = new();

            try
            {
                await Task.Run(() =>
                {
                    JsonSerializerSettings settings = new()
                    {
                        Error = (serializer, err) =>
                    {
                        err.ErrorContext.Handled = true;
                    }
                    };

                    rootObject = JsonConvert.DeserializeObject<AccessNowRequestModel.RootObject>(jsonContent, settings);
                });

                return rootObject;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return rootObject;
            }
        }
        #endregion

    }
}
