using NUnit.Framework;
using RestSharp;
using RestSharp.Serializers;
using RestSharp.Deserializers;
using Newtonsoft.Json;
using System;
namespace ApiTests
{
    public class BaseClass
    {

        protected IRestClient client;
        [SetUp]
        public void Setup()
            {
                  client = new RestClient("http://localhost:5000");
            }

        public User createUser()
        {
            var request = new RestRequest("api/createuser", Method.POST);

            string userName = "saransh" + DateTime.Now.ToFileTime() + "@piedpiper.com";

            
            request.AddParameter("Content-Type", "application.json");

            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = userName, password = "password"});
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);
           
            Assert.AreEqual(userName, obj.UserName);
            Assert.AreEqual(null, obj.Message);
            Assert.AreEqual(null, obj.Description);
            Assert.AreEqual(null, obj.Password);
            Assert.AreEqual(200, (int)res.StatusCode);
            return obj;
        }

        public User getToken(User user)
        {
            var request = new RestRequest("api/login", Method.POST);

            

            request.AddParameter("Content-Type", "application.json");

            request.AddHeader("accept", "text/plain");
            request.AddJsonBody(new { username = user.UserName, password= "password" });
            var res = client.Execute(request);

            var obj = JsonConvert.DeserializeObject<User>(res.Content);
          
            Assert.AreEqual(user.UserName, obj.UserName);
            Assert.AreEqual(null, obj.Message);
            Assert.AreEqual(null, obj.Description);
            Assert.AreEqual(null, obj.Password);
            Assert.AreEqual(200, (int)res.StatusCode);
            return obj;
        }
    }
}
