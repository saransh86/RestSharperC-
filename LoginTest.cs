using NUnit.Framework;
using RestSharp;
using RestSharp.Serializers;
using RestSharp.Deserializers;
using Newtonsoft.Json;
namespace ApiTests
{
    public class LoginTest : BaseClass
    {
        

        [Test]
        public void TestValidLogin()
        {
            
            var request = new RestRequest("api/login", Method.POST);

            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new {username = "richard@piedpiper.com", password = "password" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);
 
            Assert.AreEqual("richard@piedpiper.com", obj.UserName);
            Assert.AreEqual(null, obj.Message);
            Assert.AreEqual(null, obj.Description);
            Assert.AreEqual(null, obj.Password);
        }

        [Test]
        public void TestInvalidLogin()
        {
    
            var request = new RestRequest("api/login", Method.POST);

            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = "richard1@piedpiper.com", password = "password" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);
       
            Assert.AreEqual("bad user", obj.Message);
            Assert.AreEqual("user not found", obj.Description);

        }

        [Test]
        public void TestNoUsername()
        {
            
            var request = new RestRequest("api/login", Method.POST);

            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");

            string username = null;
            request.AddJsonBody(new { username = username, password = "password" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);

            Assert.AreEqual("bad user", obj.Message);
            Assert.AreEqual("user not found", obj.Description);
        }

        [Test]
        public void TestNoPassword()
        {
           // var client = new RestClient("http://localhost:5000");
            var request = new RestRequest("api/login", Method.POST);

            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = "richard1@piedpiper.com", password = "" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);

            Assert.AreEqual("bad user", obj.Message);
            Assert.AreEqual("user not found", obj.Description);
        }
       
    }
}