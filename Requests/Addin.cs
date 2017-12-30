using System;
using System.Collections.Generic;
using ExcelDna.Integration;
using Requests.Providers;
using Requests.Models;
using NetOffice.ExcelApi;
using System.Runtime.InteropServices;
using Requests.UI;

namespace Requests
{
    [ComVisible(true)]
    public class Addin: IExcelAddIn
    {
        private static Cache cache = new Cache();
        private static HttpProvider httpProvider = new HttpProvider();
        private static Application application;
        private static Workbook workbook;
        private static string DefaultMetaPrefix = "__meta";

        public void AutoOpen()
        {
            application = new Application(null, ExcelDnaUtil.Application);
            application.WorkbookActivateEvent += Application_WorkbookActivate;
        }

        private void Application_WorkbookActivate(Workbook Wb)
        {
            workbook = Wb;
            workbook.SheetBeforeRightClickEvent += Workbook_SheetBeforeRightClick;
        }

        private void Workbook_SheetBeforeRightClick(object Sh, Range Target, ref bool Cancel)
        {
            var caption = "Show Json...";
            var contextMenu = application.CommandBars["Cell"];

                var createControl = true;
            var cellValue = Target.Cells[1, 1].Value?.ToString();
            var showMenuControl = false;
            if (Target.Cells.Count == 1 && cellValue != null)
            {
                showMenuControl = (cellValue as string).StartsWith("http://") || (cellValue as string).StartsWith("https://");
            }

            foreach (var control in contextMenu.Controls)
            {
                if (control.Caption == caption)
                {
                    if (!showMenuControl)
                        control.Delete();
                    else
                        createControl = false;
                }
            }

            if (!(createControl && showMenuControl))
                return;

            var v = Target.Value;
            var menuItem = contextMenu.Controls.Add();
            menuItem.Caption = caption;
            menuItem.BeginGroup = true;
            menuItem.OnAction = "ShowJson";
        }


        [ExcelCommand(Name = "ShowJson")]
        public static void ShowJson()
        {
            var url = application.ActiveCell.Value.ToString();
            var route = new Route(DefaultMetaPrefix, url, null);
            var response = GetFromCache(route.Url);
            var jToken = route.IsMeta ? response.Meta : response.Json;

            var viewer = new JsonViewer(url, jToken);
            viewer.Show();
        }


        public void AutoClose()
        {
        }


        public static void GetFromSourceIfNotInCache(string url, object headers)
        {
            var httpHeaders = headers is ExcelMissing ?
                new Dictionary<string, string>() :
                ExcelParams.AsDictionary<string>(headers as object[,]);

            if (!cache.ContainsKey(url))
                cache.Set(url, httpProvider.Get(url, httpHeaders));
        }

        [ExcelFunction(Name = "REQUESTS.GET")]
        public static object Get(string url, object fragment, object headers, object timeout, object allowRedirects)
        {
            try
            {
                fragment = fragment is ExcelMissing ? null : fragment;
                var route = new Route(DefaultMetaPrefix, url, fragment);

                if (ExcelDnaUtil.IsInFunctionWizard() && !cache.ContainsKey(route.Url))
                    return "";

                GetFromSourceIfNotInCache(route.Url, headers);

                var response = GetFromCache(route.Url);
                var jToken = route.IsMeta? JTokenAccessor.Get(response.Meta, route.Fragment) : JTokenAccessor.Get(response.Json, route.Fragment);
                return ExcelRenderer.Render(jToken, route, true);
                
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        [ExcelFunction(Name = "REQUESTS.LIST")]
        public static object List(string url, object fragment, object headers, object timeout, object allowRedirects, object traverseObject)
        {
            try
            {
                fragment = fragment is ExcelMissing ? null : fragment;
                var route = new Route(DefaultMetaPrefix, url, fragment);

                if (ExcelDnaUtil.IsInFunctionWizard() && !cache.ContainsKey(route.Url))
                    return "";

                GetFromSourceIfNotInCache(route.Url, headers);

                var response = GetFromCache(route.Url);
                var jToken = route.IsMeta ? response.Meta : response.Json;
                var properties = JTokenAccessor.Properties(jToken, route.Fragment);
                var results = new object[properties.Count, 2];
                for (int i = 0; i < properties.Count; i++)
                {
                    results[i, 0] = route.Url + "#" + (route.IsMeta? DefaultMetaPrefix + "/" : "") + properties[i].Path;
                    results[i, 1] = properties[i].Type;
                }
                return results;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [ExcelFunction(Name = "REQUESTS.TYPE")]
        public static object Type(string url, object fragment, object headers, object authentication, object timeout, object allowRedirects)
        {
            try
            {
                fragment = fragment is ExcelMissing ? null : fragment;
                var route = new Route(DefaultMetaPrefix, url, fragment);
                if (ExcelDnaUtil.IsInFunctionWizard() && !cache.ContainsKey(route.Url))
                    return "";
                GetFromSourceIfNotInCache(route.Url, headers);
                var value = GetFromCache(route.Url).Json;
                return value.Type.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [ExcelFunction(Name = "REQUESTS.FLUSH")]
        public static object Flush(object keys)
        {
            try
            {
                int count = 0;
                if (keys is ExcelMissing)
                {
                    count = cache.Count;
                    cache.Flush();
                }
                else
                {
                    foreach (var key in (keys as string[]))
                    {
                        cache.Flush(key);
                        count++;
                    }
                }
                return String.Format("Flushed {0} key(s)", count);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static Response GetFromCache(string url)
        {
            var schema = new Schema(url);
            return cache.Get(schema.Base);
        }

    }
}
