using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Requests.Models;

namespace Requests.Providers
{
    public class HttpProvider
    {
        public Response Get(string url, Dictionary<string, string> headers)
        {
            var httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            var httpResponse = httpClient.SendAsync(request).Result;
            IEnumerable<string> contentTypeAsIEnumerable;
            httpResponse.Content.Headers.TryGetValues("Content-Type", out contentTypeAsIEnumerable);
            var contentType = contentTypeAsIEnumerable?.First();

            var metaData = new Dictionary<string, object>()
            {
                { "Success", httpResponse.IsSuccessStatusCode},
                { "StatusCode", (int)httpResponse.StatusCode },
                { "Headers", httpResponse.Headers.ToDictionary(x => x.Key, x => x.Value) }
            };

            var response = new Response()
            {
                Meta = JObject.FromObject(metaData)
            };

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var text = httpResponse.Content.ReadAsStringAsync().Result;
                response.Text = text;
                if (contentType != null && contentType.ToLower().Contains("application/json"))
                    response.Json = JsonParser.Parse(text);
            }
            return response;
        }

        public Response Post(string url, Dictionary<string, string> headers, JToken payload)
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

            var meta = new JObject();
            response.Add("Meta", meta);
            using (var httpResponse = request.GetResponse() as HttpWebResponse)
            {
                meta.Add("Text", JValue.CreateNull());
                meta.Add("StatusCode", (int)httpResponse.StatusCode);
                meta.Add("StatusDescription", httpResponse.StatusDescription);
                meta.Add("ContentType", httpResponse.ContentType);
                meta.Add("Method", httpResponse.Method);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                    {
                        var text = reader.ReadToEnd();
                        meta["Text"] = text;
                        if (httpResponse.ContentType.ToLower().Contains("application/json"))
                            response.Add("Json", JsonParser.Parse(text));
                    }
                }
            }
            return null;// response;
        }
    }
}
