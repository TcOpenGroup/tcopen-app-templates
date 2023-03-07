using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortex.Connector;
using System.Windows;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AutoMapper;
using System.IO;
using System.Net;

namespace x_template_xPlc
{
    public partial class TcoSmartFunctionKit
    {
        public void InitializeTask()
        {
            this.__getResultsTask.InitializeExclusively(GetResults);
        }

        private void GetResults()
        {
            var client = new Client("192.168.0.1");

           var curve = client.GetLastCurveData();
            _results.createdDate.Cyclic = curve.createdDate;
            _results.customId.Cyclic = curve.customId;
            _results.cycleTime .Cyclic = Convert.ToSingle(curve.cycleTime);
            _results.dataRecordingDisabled .Cyclic = curve.dataRecordingDisabled;
            _results.id.Cyclic = curve.id;
            _results.maxForce.Cyclic = Convert.ToSingle(curve.maxForce);
            _results.maxPosition .Cyclic = Convert.ToSingle(curve.maxPosition);
            _results.samplingInterval.Cyclic =(short) curve.samplingInterval;
            _results.status .Cyclic = curve.status;
            _results.valid .Cyclic = curve.valid;
            _results.validationTime.Cyclic =(short) curve.validationTime;
            _results._v.Cyclic = (short)curve.__v;
            this.Write();

        }

        private bool Save()
        {
            //if (uid.Length == 0)
            //{
            //    Console.WriteLine( "Provided UID is blank!");
            //    return false;
            //}

            //string dateStamp = DateTime.Now.ToString(".yyyy.MM.dd.HH.mm.ss");
            //string FileName = uid + dateStamp.Replace(".", "_") + ".json";
            //string dirName = DateTime.Now.ToString("yyyy.MM.dd");
            //string FilePath = Path.Combine(saveDir, dirName, FileName);
            string json = "";

            //Directory.CreateDirectory(Path.Combine(saveDir, dirName));

            try
            {
                using (WebClient wc = new WebClient())
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
                    string uri = "https://192.168.0.1/api/curves?page=0&size=1&sort={" + " \"_id \":\"desc\"}";
                    Uri url = new Uri(uri);
                    json = wc.DownloadString(url);
                }
            }
            catch (Exception e)
            {

                //   MessageBox.Show(e.Message);
               Console.WriteLine( e.Message);
            }

            //if (json.Length == 0)
            //{
            //    this.Status.ErrorMessage.Cyclic = "Curve .json file contains no data!";
            //    return false;
            //}

            //File.WriteAllText(FilePath, json);
            return true;
        }
    }

}
