using NUnit.Framework;


namespace Requests.Test
{
    class SchemaTests
    {
        [Test]
        public void CanParseUrlWithoutHash()
        {
            var url = "http://api.test.com/";
            var schema = new Schema(url);
            Assert.AreEqual("http://api.test.com", schema.Base); //removes trailing slash
            Assert.IsNull(schema.Path);
        }


        [Test]
        public void CanParseUrlWithHash()
        {
            var url = "http://api.test.com#path/to/thingy";
            var schema = new Schema(url);
            Assert.AreEqual("http://api.test.com", schema.Base);
            Assert.AreEqual(schema.Path, "path/to/thingy");
        }

    }
}
