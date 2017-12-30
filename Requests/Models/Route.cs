
namespace Requests.Models
{
    public class Route
    {
        public bool IsMeta { get; private set; }
        public string Url;
        public string Fragment;
        public string MetaPrefix { get; private set; }

        public static string Combine(string fragment1, string fragment2)
        {
            if(fragment1 == null)
                return fragment2;

            return fragment1 + "/" + fragment2;
        }

        public Route(string metaPrefix, string url, object fragment)
        {
            url = Trim(url);
            MetaPrefix = metaPrefix;

            if (fragment == null && url.Contains("#"))
            {
                var urlAndProperty = url.Split('#');
                Url = urlAndProperty[0];
                fragment = urlAndProperty[1];
            }
            else
            {
                Url = url;
            }

            if (fragment != null)
            {
                Fragment = fragment.ToString();
                if (Fragment.StartsWith(MetaPrefix))
                {
                    Fragment = Fragment == MetaPrefix? null : Fragment.Substring(MetaPrefix.Length + 1);
                    IsMeta = true;
                }
            }
            else
            {
                Fragment = null;
            }
        }

        public Route(string metaPrefix, string url) : this(metaPrefix, url, null)
        {
        }

        private string Trim(string url)
        {
            return url.TrimEnd(new char[] { '/' });
        }

        public string Render()
        {
            if (Fragment == null)
            {
                if (IsMeta)
                    return Url + "#" + MetaPrefix;
                return Url;
            }

            return Url + "#" + (IsMeta? MetaPrefix + "/" : "") + Fragment;
        }


        public string Render(string path)
        {
            if (Fragment == null)
            {
                if (IsMeta)
                    return Url + "#" + MetaPrefix;
                return Url;
            }

            return Url + "#" + (IsMeta ? MetaPrefix + "/" : "") + Fragment + "/" + path;
        }


    }
}
