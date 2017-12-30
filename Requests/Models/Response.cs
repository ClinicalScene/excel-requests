using Newtonsoft.Json.Linq;

namespace Requests.Models
{
    public class Response
    {
        public string Text;
        public bool Success;
        public int StatusCode;
        public string StatusDescription;
        public string ContentType;
        public string Method;
        public JToken Meta;
        public JToken Json;
    }
}
