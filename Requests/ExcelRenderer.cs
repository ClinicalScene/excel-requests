using System;
using Newtonsoft.Json.Linq;
using Requests.Models;

namespace Requests
{
    public class ExcelRenderer
    {

        public static object Render(JToken token, Route route, bool traverse)
        {
            if (token.Type == JTokenType.Integer)
                return token.ToObject<int>();

            if (token.Type == JTokenType.Float)
                return token.ToObject<double>();

            if (token.Type == JTokenType.String)
                return token.ToObject<string>();

            if (token.Type == JTokenType.Boolean)
                return token.ToObject<bool>();

            if (token.Type == JTokenType.Date)
                return token.ToObject<DateTime>();

            if (token.Type == JTokenType.Array)
            {
                if (!traverse)
                    return route.Render();

                int columns = 1;
                int rows = (token as JArray).Count;

                foreach (var o in token as JArray)
                {
                    if (o is JArray)
                        columns = Math.Max((o as JArray).Count, columns);
                }

                var array = new object[rows, columns];

                for(int i=0; i < rows; i++)
                {
                    if ((token as JArray)[i] is JArray)
                    {
                        var arr = (token as JArray)[i] as JArray;
                        var j = 0;
                        foreach (var item in arr)
                        {
                            array[i, j] = Render(item, 
                                new Route(route.MetaPrefix, route.Url, Route.Combine(route.Fragment, i + "/" + j)), false);
                            j++;
                        }
                    }
                    else
                    {
                        array[i, 0] = Render((token as JArray)[i],
                            new Route(route.MetaPrefix, route.Url, Route.Combine(route.Fragment, i.ToString())), false);
                    }
                }
                return array;
            }

            return route.Render();

        }
    }
}
