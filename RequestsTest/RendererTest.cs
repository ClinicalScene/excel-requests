using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Requests;
using Requests.Models;

namespace Requests.Test
{
    class RendererTests
    {
        [Test]
        public void CanRenderSimpleArray()
        {
            var token = JsonParser.Parse(@"[1,2,3,4,5,6]");            
            var result = ExcelRenderer.Render(token, new Route("__meta", "http://api.test.com/numbers"), true);
            Assert.AreEqual(new object[,] { { 1 }, { 2 }, { 3 }, { 4 }, { 5 }, { 6 } }, result);
        }


        [Test]
        public void CanRenderArrayOfObjects()
        {
            var token = JsonParser.Parse(@"[1,2, {""a"": 5}]");
            var result = ExcelRenderer.Render(token, new Route("__meta", "http://api.test.com/numbers"), true);
            Assert.AreEqual(new object[,] { { 1 }, { 2 }, { "http://api.test.com/numbers#2" } }, result);
        }




        [Test]
        public void CanRenderArrayOfArrays()
        {
            var token = JsonParser.Parse(@"[[1,2,3],[4,5,6]]");
            var result = ExcelRenderer.Render(token, new Route("__meta", "http://api.test.com/numbers"), true);
            Assert.AreEqual(new object[,] { { 1 , 2 , 3 }, { 4, 5, 6 } }, result);
        }


        [Test]
        public void CanRenderDateTime()
        {
            var token = JsonParser.Parse(@"{""created_at"": ""2016-09-13T20:40:41Z""}");
            var result = ExcelRenderer.Render(token["created_at"], new Route("__meta", "http://api.test.com/numbers"), true);
            Assert.AreEqual(new DateTime(2016, 9, 13, 20, 40, 41), result);
        }
    }
}
