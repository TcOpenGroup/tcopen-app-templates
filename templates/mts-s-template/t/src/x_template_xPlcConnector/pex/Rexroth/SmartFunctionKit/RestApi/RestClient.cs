using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;


namespace x_template_xPlc
{
    public class Client
    {
        public class RestClient : RestRequest
        {
            public RestClient(string s)
            {
                base.ResourcePath = s;
            }
        }

        public Client(string address)
        {
            m_address = address;
            m_uri = "https://" + address + "/api";
        }
        public string m_uri;
        public string m_address;
        private readonly string _userName;
        private readonly string _password;
        private static Logger m_logger = Logger.Create(typeof(RestRequest));

        public RestClient Api(string s)
        {
            return new RestClient(m_uri + "/" + s);
        }


        public CurveItem GetLastCurveData()
        {


            var resp = this.Api("curves?page=0&size=1&sort={\"_id\":\"desc\"}").Get();
            var removeRootBracket = resp.Substring(1, resp.Length - 2);
            var curve = JsonConvert.DeserializeObject<CurveItem>(removeRootBracket);

            return curve;
        }

       


        //private void SaveCurve(string uid)
        //{
        //    if (uid.Length == 0)
        //    {

        //        return ;
        //    }

        //    string dateStamp = DateTime.Now.ToString(".yyyy.MM.dd.HH.mm.ss");
        //    string FileName = uid + dateStamp.Replace(".", "_") + ".json";
        //    string dirName = DateTime.Now.ToString("yyyy.MM.dd");
        //    string FilePath = Path.Combine(saveDir, dirName, FileName);
        //    string json = "";

        //    Directory.CreateDirectory(Path.Combine(saveDir, dirName));

        //    try
        //    {
        //        using (WebClient wc = new WebClient())
        //        {
        //            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
        //            Uri url = new Uri("https://" + ipAddress + "/api/curves/" + uid);
        //            json = wc.DownloadString(url);
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        //   MessageBox.Show(e.Message);
        //        this.Status.ErrorMessage.Cyclic = e.Message;
        //    }

        //    if (json.Length == 0)
        //    {
        //        this.Status.ErrorMessage.Cyclic = "Curve .json file contains no data!";
        //        return false;
        //    }

        //    File.WriteAllText(FilePath, json);
        //    return true;
        //}
    }
}

     


