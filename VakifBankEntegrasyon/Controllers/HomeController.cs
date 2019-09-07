using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;

namespace VakifBankEntegrasyon.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {


        
            string data = "Pan=4289450189088488&ExpiryDate=2304&PurchaseAmount=5.00&Currency=840&BrandName=100&VerifyEnrollmentRequestId=" + Guid.NewGuid().ToString("N") + "&SessionInfo=&MerchantID=000100000013506&MerchantPassword=123456&SuccessUrl=http://localhost:52678/Home/Geldi"; //replace <value>
            byte[] dataStream = Encoding.UTF8.GetBytes(data);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("https://3dsecuretest.vakifbank.com.tr:4443/MPIAPI/MPI_Enrollment.aspx"); //Mpi Enrollment Adresi
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = dataStream.Length;
            webRequest.KeepAlive = false;
            string responseFromServer = "";

            using (Stream newStream = webRequest.GetRequestStream())
            {
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseFromServer = reader.ReadToEnd();
                    reader.Close();
                }

                webResponse.Close();
            }

            if (string.IsNullOrEmpty(responseFromServer))
            {
                int a=0;
               
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(responseFromServer);


            var statusNode = xmlDocument.SelectSingleNode("IPaySecure/Message/VERes/Status");
            var pareqNode = xmlDocument.SelectSingleNode("IPaySecure/Message/VERes/PaReq");
            var acsUrlNode = xmlDocument.SelectSingleNode("IPaySecure/Message/VERes/ACSUrl");
            var termUrlNode = xmlDocument.SelectSingleNode("IPaySecure/Message/VERes/TermUrl");
            var mdNode = xmlDocument.SelectSingleNode("IPaySecure/Message/VERes/MD");
            var messageErrorCodeNode = xmlDocument.SelectSingleNode("IPaySecure/MessageErrorCode");

            string statusText = "";

            if (statusNode != null)
            {
                statusText = statusNode.InnerText;
            }

            //3d secure programına dahil
            if (statusText == "Y")
            {
                string postBackForm =
                   @"<html>
                          <head>
                            <meta name=""viewport"" content=""width=device-width"" />
                            <title>MpiForm</title>
                            <script>
                              function postPage() {
                              document.forms[""frmMpiForm""].submit();
                              }
                            </script>
                          </head>
                          <body onload=""javascript:postPage();"">
                            <form action=""@ACSUrl"" method=""post"" id=""frmMpiForm"" name=""frmMpiForm"">
                              <input type=""hidden"" name=""PaReq"" value=""@PAReq"" />
                              <input type=""hidden"" name=""TermUrl"" value=""@TermUrl"" />
                              <input type=""hidden"" name=""MD"" value=""@MD "" />
                              <noscript>
                                <input type=""submit"" id=""btnSubmit"" value=""Gönder"" />
                              </noscript>
                            </form>
                          </body>
                        </html>";

                postBackForm = postBackForm.Replace("@ACSUrl", acsUrlNode.InnerText);
                postBackForm = postBackForm.Replace("@PAReq", pareqNode.InnerText);
                postBackForm = postBackForm.Replace("@TermUrl", termUrlNode.InnerText);
                postBackForm = postBackForm.Replace("@MD", mdNode.InnerText);

                Response.ContentType = "text/html";
                Response.Write(postBackForm);
            }
            else if (statusText == "E")
            {
                string errorCode = messageErrorCodeNode.InnerText;
            }
            return View();
        }
        

        public ActionResult Geldi()
        {
           string Status = Request.Form["Status"];
            string MerchantId = Request.Form["MerchantId"];
            string VerifyEnrollmentRequestId = Request.Form["VerifyEnrollmentRequestId"];
            string Xid = Request.Form["Xid"];
            string PurchAmount = Request.Form["PurchAmount"];           
            string SessionInfo = Request.Form["SessionInfo"];
            string PurchCurrency = Request.Form["PurchCurrency"];
            string Pan = Request.Form["Pan"];
            string ExpiryDate = Request.Form["Expiry"];
            string Eci = Request.Form["Eci"];
            string Cavv = Request.Form["Cavv"];
            string InstallmentCount = Request.Form["InstallmentCount"];

            return View();
        }
       
    }
}