using NUnit.Framework;


namespace Requests.Test
{
    class JTokenTests
    {
        string jsonList;

        [SetUp]
        public void SetUp()
        {
            jsonList = @"[
                {
                    postId: 1,
                    id: 1,
                    name: ""id labore ex et quam laborum"",
                    email: ""Eliseo@gardner.biz"",
                    body: ""laudantium enim quasi est quidem magnam voluptate ipsam eos tempora quo necessitatibus dolor quam autem quasi reiciendis et nam sapiente accusantium""
                },
                {
                    postId: 1,
                    id: 2,
                    name: ""quo vero reiciendis velit similique earum"",
                    email: ""Jayne_Kuhic@sydney.com"",
                    body: ""est natus enim nihil est dolore omnis voluptatem numquam et omnis occaecati quod ullam at voluptatem error expedita pariatur nihil sint nostrum voluptatem reiciendis et""
                }]";
        }

        [Test]
        public void CanListProperties()
        {
            var token = JsonParser.Parse(jsonList);
            var properties = JTokenAccessor.Properties(token);
            Assert.AreEqual(properties.Count, 2);
            Assert.AreEqual(properties[0].Path, "0");
            Assert.AreEqual(properties[1].Path, "1");

            Assert.AreEqual(properties[0].Type, "Object");
            Assert.AreEqual(properties[1].Type, "Object");
        }

        [Test]
        public void CanListItemProperties()
        {
            var token = JsonParser.Parse(jsonList);
            var properties = JTokenAccessor.Properties(token, "0");
            Assert.AreEqual(properties.Count, 5);
            Assert.AreEqual(properties[0].Path, "0/postId");
            Assert.AreEqual(properties[1].Path, "0/id");
            Assert.AreEqual(properties[2].Path, "0/name");
            Assert.AreEqual(properties[3].Path, "0/email");
            Assert.AreEqual(properties[4].Path, "0/body");

            Assert.AreEqual(properties[0].Type, "Integer");
            Assert.AreEqual(properties[1].Type, "Integer");
            Assert.AreEqual(properties[2].Type, "String");
            Assert.AreEqual(properties[3].Type, "String");
            Assert.AreEqual(properties[4].Type, "String");
        }

    }
}
