﻿using Newtonsoft.Json.Linq;


namespace Requests
{
    public class Parser
    {
        public static JToken Parse(string json)
        {
            if (json.StartsWith("["))
                return JArray.Parse(json);

            return JObject.Parse(json);
        }
    }
}
