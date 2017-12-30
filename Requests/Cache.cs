using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using Requests.Providers;
using Requests.Models;

namespace Requests
{
    public class Cache
    {
        private Dictionary<string, Response> objects = new Dictionary<string, Response>();
        private HttpProvider httpProvider = new HttpProvider();


        public int Count
        {
            get
            {
                return objects.Count;
            }
        }

        public void Flush()
        {
            objects.Clear();
        }

        public void Flush(string key)
        {
            if (objects.ContainsKey(key))
                objects.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return objects.ContainsKey(key);
        }


        public Response Get(string key)
        {
            return objects[key];
        }

        public Response Set(string key, Response response)
        {
            objects[key] = response;
            return response;
        }


    }
}
