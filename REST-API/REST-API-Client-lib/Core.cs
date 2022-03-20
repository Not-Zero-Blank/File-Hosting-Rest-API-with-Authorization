using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace REST_API_Client_lib
{
    public class Files
    {
        public static string Upload(byte[] data, string FileName, string Domain = "localhost:5001")
        {
            var url = $"https://{Domain}/Upload?FileName={FileName}";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            var uploaddata = "\""+Convert.ToBase64String(data)+ "\"";
            data = new byte[0];
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(uploaddata);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
        public static byte[] DownloadBytes(string FileName, string Domain = "localhost:5001")
        {
            var url = $"https://{Domain}/Download?FileName={FileName}";
            return new WebClient().DownloadData(url);
        }
        public static void DownloadToPath(string Path, string FileName, string Domain = "localhost:5001")
        {
            var url = $"https://{Domain}/Download?FileName={FileName}";
            new WebClient().DownloadFile(url, Path+"/"+FileName.Split('\\').Last());
        }
        public static string[] GetAllFiles(string Username, string Password, string Domain = "localhost:5001")
        {
            var url = $"https://{Domain}/Files?Username={Username}&Password={Password}";
            return new WebClient().DownloadString(url).Split('\n');
        }
        public static string DeleteFile(string Username, string Password, string FileName, string Domain = "localhost:5001")
        {
            var url = $"https://{Domain}/Delete?Username={Username}&Password={Password}&FileName={FileName}";
            return new WebClient().DownloadString(url);
        }
    }
    public class User
    {
        public static string CreateRegisterKey(string Username, string Password, string Domain = "localhost:5001")
        {
            var url = $"https://{Domain}/CreateRegisterKey?Username={Username}&Password={Password}";
            return new WebClient().UploadString(url, "");
        }
        public static string Register(string Username, string Password, string RegisterKey, string Domain = "localhost:5001")
        {
            var url = $"https://{Domain}/Register?Username={Username}&Password={Password}&Key={RegisterKey}";
            return new WebClient().UploadString(url, "");
        }
    }
}
