using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using Requests.Models;


namespace Requests
{
    public class JTokenAccessor
    {
        private static char delimiter = '/';

        private static JToken get(object o, string key)
        {
            if (o is JArray)
                return (o as JArray)[Int32.Parse(key)];

            if (o is JObject)
                return (o as JObject)[key];

            return null;

        }


        public static JToken Get(JToken jToken, string path)
        {
            if (path == null)
                return jToken;
            var parts = path.Split(delimiter);
            var o = jToken;
            foreach (var part in parts)
            {
                o = get(o, part);
            }
            return o;
        }


        public static JToken Set(JToken jToken, string path, JToken value)
        {
            var parts = path.Split(delimiter);
            var count = parts.Length;
            var o = jToken;
            for(var i=0; i<count; i++)
            {
                if (i < count - 1)
                {
                    o[parts[i]] = new JObject();
                    o = o[parts[i]];
                }
                else
                {
                    o[parts[i]] = value;
                }
            }
            return value;
        }


        public static IList<Property> Properties(JToken token)
        {
            return Properties(token, null);

        }

        public static IList<Property> Properties(JToken jToken, string path)
        {
            var item = path == null ? jToken : Get(jToken, path);

            if (!(item is JObject || item is JArray))
                throw new Exception(String.Format("Invalid token type, expected Array or Object"));

            var paths = item is JObject ?
                (item as JObject).Properties().ToList().Select(x => (path == null ? "" : path + "/") + x.Name).ToList()
                : Enumerable.Range(0, (item as JArray).Count).Select(x => (path == null ? "" : path + "/") + x.ToString()).ToList();

            return paths.Select(p => new Property()
            {
                Path = p,
                Type = Get(jToken, p).Type.ToString()
            }).ToList();
        }


    }
}
