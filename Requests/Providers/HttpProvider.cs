using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace Requests.Providers
{
    public class HttpProvider
    {
        public JToken Get(string url, Dictionary<string, string> headers)
        {
            var httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            var response = httpClient.SendAsync(request).Result;
            IEnumerable<string> contentTypeAsIEnumerable;
            response.Content.Headers.TryGetValues("Content-Type", out contentTypeAsIEnumerable);
            var contentType = contentTypeAsIEnumerable?.First();

            var payload = new JObject();

            payload.Add("Text", JValue.CreateNull());
            payload.Add("StatusCode", (int)response.StatusCode);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var text = response.Content.ReadAsStringAsync().Result;
                payload["Text"] = text;
                if(contentType != null && contentType.ToLower().Contains("application/json"))
                    payload.Add("Json", Parser.Parse(text));
            }
            return payload;
        }

        public JToken Post(string url, Dictionary<string, string> headers, JToken payload)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var request = WebRequest.Create(url) as HttpWebRequest;
            var response = new JObject();
            request.ProtocolVersion = HttpVersion.Version10;
            request.KeepAlive = false;
            request.ServicePoint.Expect100Continue = false;
            request.UserAgent = "excel-requests";

            var data = Encoding.ASCII.GetBytes(payload.ToString(Newtonsoft.Json.Formatting.None));
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            using (var httpResponse = request.GetResponse() as HttpWebResponse)
            {
                response.Add("Text", JValue.CreateNull());
                response.Add("StatusCode", (int)httpResponse.StatusCode);
                response.Add("StatusDescription", httpResponse.StatusDescription);
                response.Add("ContentType", httpResponse.ContentType);
                response.Add("Method", httpResponse.Method);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                    {
                        var text = reader.ReadToEnd();
                        response["Text"] = text;
                        if (httpResponse.ContentType.ToLower().Contains("application/json"))
                            response.Add("Json", Parser.Parse(text));
                    }
                }
            }
            return response;
        }
    }
}
