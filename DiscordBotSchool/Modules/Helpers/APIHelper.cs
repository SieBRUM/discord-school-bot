using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

namespace DiscordBotSchool.Modules.Helpers
{
    public static class APIHelper
    {
        const string BASE_URL = "http://localhost:8080/backend/api/";

        public static string MakePostCall<T>(string endpoint, T item)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(BASE_URL + endpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
            httpWebRequest.ContentLength = byteArray.Length;
            Stream dataStream = httpWebRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = httpWebRequest.GetResponse();

            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }

        public static string MakeGetRequest(string endpoint)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(BASE_URL + endpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            WebResponse response = httpWebRequest.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }
    }
}
