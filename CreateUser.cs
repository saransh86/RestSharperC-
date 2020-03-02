using NUnit.Framework;
using RestSharp;
using RestSharp.Serializers;
using RestSharp.Deserializers;
using Newtonsoft.Json;
using System;

namespace ApiTests
{
    /**
     * These tests can potentially hog DB space. We would definately need a delete user to clean up.
     */
    public class CreateUserTest : BaseClass
    {
        [Test]
        public void TestCreateUser()
        {
            var request = new RestRequest("api/createuser", Method.POST);

            string userNAme = "saransh" + DateTime.Now.ToFileTime() + "@piedpiper.com";

            request.AddParameter("Content-Type", "application.json");

            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = userNAme, password = "password" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);
          
            Assert.AreEqual(userNAme, obj.UserName);
            Assert.AreEqual(null, obj.Message);
            Assert.AreEqual(null, obj.Description);
            Assert.AreEqual(null, obj.Password);
            Assert.AreEqual(200, (int)res.StatusCode);
        }

        [Test]
        public void TestCreateUserWithNullUsername()
        {
            var request = new RestRequest("api/createuser", Method.POST);

            request.AddParameter("Content-Type", "application.json");
            string userName = null;
            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = userName, password = "password" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);
            Assert.AreEqual("Object reference not set to an instance of an object.", obj.Message);
            Assert.AreEqual(500, (int)res.StatusCode);

        }

        [Test]
        public void TestCreateUserWithNoUsername()
        {
            var request = new RestRequest("api/createuser", Method.POST);

            request.AddParameter("Content-Type", "application.json");

            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = "", password = "password" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);
            Assert.AreEqual(null, obj.Message);
            Assert.AreEqual(200, (int)res.StatusCode);
        }
        /**
         * This test should Ideally fail, user should not be created with a empty password
         */
        [Test]

        public void TestCreateUserWithNoPassword()
        {
            var request = new RestRequest("api/createuser", Method.POST);
            string userNAme = "saransh" + DateTime.Now.ToFileTime() + "@piedpiper.com";
            request.AddParameter("Content-Type", "application.json");

            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = userNAme, password = "" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);
            Assert.AreEqual(userNAme, obj.UserName);
            Assert.AreEqual(null, obj.Message);
            Assert.AreEqual(null, obj.Description);
            Assert.AreEqual(null, obj.Password);
            Assert.AreEqual(200, (int)res.StatusCode);
        }
        [Test]

        public void TestCreateUserWithNullPassword()
        {
            var request = new RestRequest("api/createuser", Method.POST);
            string userNAme = "saransh" + DateTime.Now.ToFileTime() + "@piedpiper.com";
            request.AddParameter("Content-Type", "application.json");
            string password = null;
            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = userNAme, password = password });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);
            Assert.AreEqual("Value cannot be null.\nParameter name: password", obj.Message);
            Assert.AreEqual(500, (int)res.StatusCode);
        }

        [Test]
        public void TestCreateSameUser()
        {
            var request = new RestRequest("api/createuser", Method.POST);

            string userNAme = "saransh" + DateTime.Now.ToFileTime() + "@piedpiper.com";

            request.AddParameter("Content-Type", "application.json");

            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = userNAme, password = "password" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);

            Assert.AreEqual(userNAme, obj.UserName);
            Assert.AreEqual(null, obj.Message);
            Assert.AreEqual(null, obj.Description);
            Assert.AreEqual(null, obj.Password);
            Assert.AreEqual(200, (int)res.StatusCode);
            /**
             * Create the same user again
             */
            var resSameUser = client.Execute(request);
            var objSameUser = JsonConvert.DeserializeObject<User>(resSameUser.Content);
            Assert.AreEqual("An error occurred while updating the entries. See the inner exception for details.", objSameUser.Message);
            Assert.AreEqual(500, (int)resSameUser.StatusCode);

        }

    }
}
