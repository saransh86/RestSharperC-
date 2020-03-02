using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json;
namespace ApiTests
{
  
    public class ToDoTests : BaseClass
    {
       
        User user;
        User userDetails;
        [OneTimeSetUp]
        public void init()
        {
            /**
             * Create one user and get the token to reuse for all tests
             */ 
            client = new RestClient("http://localhost:5000");
            user = createUser();
            /**
             * Get the token
             */
            userDetails = getToken(user);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            /**
             * Need the delete user api to cleanup the DB
             * Gilfoyle will not be pleased with DB space utilized :)
             */
            
        }

        [Test]
        public void TestPostValidTodoCheckAndDelete()
        {
            /**
             * Make a new Todo
             */ 
            var request = new RestRequest("api/todo", Method.POST);
            request.AddParameter("Content-Type", "application/json");
            request.AddHeader("accept", "text/plain");

           
            request.AddJsonBody(new {  user = user.UserName, title = "This is a test", body = "The description of the todo", Checked = false });
            request.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var res = client.Execute(request);

            /**
             * Make sure we got it posted
             */
            var obj = JsonConvert.DeserializeObject<ToDo>(res.Content);
            Assert.AreEqual(200, (int)res.StatusCode);
            Assert.AreEqual("This is a test", obj.title);
            Assert.AreEqual("The description of the todo", obj.body);
            Assert.IsFalse(obj.Checked);

            /**
             * Get the todo we just posted
             */
            var reqGetTodos = new RestRequest("api/todo/" + obj.uid ,Method.GET);
            reqGetTodos.AddHeader("Authorization", "Bearer " + userDetails.Token);
            var resGetTodos = client.Execute(reqGetTodos);
           
            var objGetTodos = JsonConvert.DeserializeObject<ToDo>(resGetTodos.Content);
            Assert.AreEqual(200, (int)resGetTodos.StatusCode);
            Assert.AreEqual("This is a test", objGetTodos.title);
            Assert.AreEqual("The description of the todo", objGetTodos.body);
            Assert.IsFalse(objGetTodos.Checked);
            /**
             * Delete the todo
             */
            var reqDeleteTodos = new RestRequest("api/todo/" + obj.uid, Method.DELETE);
            reqDeleteTodos.AddHeader("Authorization", "Bearer " + userDetails.Token);
            var resDelete = client.Execute(reqDeleteTodos);

           
            Assert.AreEqual(200, (int)resDelete.StatusCode);
            /**
             * Check if the delete worked
             */
            var reqGetTodoAfterDelete = new RestRequest("api/todo/" + obj.uid, Method.GET);
            reqGetTodoAfterDelete.AddHeader("Authorization", "Bearer " + userDetails.Token);
            /* reqGetTodos.AddHeader("Authorization", "Bearer " + userDetails.Token);*/
            var resGetTodoAfterDelete = client.Execute(reqGetTodoAfterDelete);
            /**
             * We are golden!
             */ 
            Assert.AreEqual(204, (int)resGetTodoAfterDelete.StatusCode);
            


        }

        [Test]
        public void TestPostTodoWithNoTitle()
        {
            var request = new RestRequest("api/todo", Method.POST);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
            String title = null;
            
            request.AddJsonBody(new {  user = user.UserName, title = title, body = "The description of the todo", Checked = false });
            request.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var res = client.Execute(request);
            var obj = JsonConvert.DeserializeObject<ToDo>(res.Content);
            Assert.AreEqual(200, (int)res.StatusCode);
            Assert.AreEqual(null, obj.title);
            Assert.AreEqual("The description of the todo", obj.body);
            Assert.IsFalse(obj.Checked);
            /**
             * Note: I'm not sure why we don't have a check in the backend to confirm if the title is present or not. 
             */
            /**
            * Clean it up
            */

            var reqDelete = new RestRequest("api/todo/" + obj.uid, Method.DELETE);
            reqDelete.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var resDelete = client.Execute(reqDelete);
            Assert.AreEqual(200, (int)resDelete.StatusCode);
        }

        [Test]
        public void TestPostTodoWithCheckedTrue()
        {
            var request = new RestRequest("api/todo", Method.POST);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
            String title = null;

            request.AddJsonBody(new {  user = user.UserName, title = title, body = "The description of the todo", Checked = true });
            request.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var res = client.Execute(request);
            var obj = JsonConvert.DeserializeObject<ToDo>(res.Content);
           
            Assert.AreEqual(200, (int)res.StatusCode);
            Assert.AreEqual(null, obj.title);
            Assert.AreEqual("The description of the todo", obj.body);
            Assert.IsTrue(obj.Checked);
            /**
             * Note: Again, I can't replicate this scenario from the frontend, but seems odd I can create a new todo and mark it checked at the same time. I would expect
             * a check in the backend too.
             */
            /**
            * Clean it up
            */

            var reqDelete = new RestRequest("api/todo/" + obj.uid, Method.DELETE);
            reqDelete.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var resDelete = client.Execute(reqDelete);
            Assert.AreEqual(200, (int)resDelete.StatusCode);


        }
        [Test]
        public void TestPostWithEmptyBody()
        {
            var request = new RestRequest("api/todo", Method.POST);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
           

            request.AddJsonBody(new { });
            request.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var res = client.Execute(request);
            var obj = JsonConvert.DeserializeObject<ToDo>(res.Content);
            Assert.AreEqual(200, (int)res.StatusCode);
            Assert.AreEqual(null, obj.title);
            Assert.AreEqual(null, obj.body);
            Assert.IsFalse(obj.Checked);
            /**
             * Clean it up
             */

            var reqDelete = new RestRequest("api/todo/" + obj.uid, Method.DELETE);
            reqDelete.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var resDelete = client.Execute(reqDelete);
            Assert.AreEqual(200, (int)resDelete.StatusCode);
        }
        [Test]
        public void TestAllTodos()
        {
            var request = new RestRequest("api/todo", Method.GET);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
            request.AddHeader("Authorization", "Bearer " + userDetails.Token);
            var res = client.Execute(request);
            Assert.AreEqual(200, (int)res.StatusCode);

        }
        [Test]
        public void TestGetTodoWithRandomUid()
        {
            var request = new RestRequest("api/todo/" + Guid.NewGuid(), Method.GET);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
            request.AddHeader("Authorization", "Bearer " + userDetails.Token);
            var res = client.Execute(request);
            Assert.AreEqual(204, (int)res.StatusCode);
        }
        [Test]
        public void TestDeleteTodoWithRandomUid()
        {
            var request = new RestRequest("api/todo/" + Guid.NewGuid(), Method.DELETE);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
            request.AddHeader("Authorization", "Bearer " + userDetails.Token);
            var res = client.Execute(request);
            var obj = JsonConvert.DeserializeObject<ToDo>(res.Content);
           
            Assert.AreEqual(404, (int)res.StatusCode);
            Assert.AreEqual("todo not found", obj.Message);
        }
        [Test]
        public void TestAccessPostTodoWithoutToken()
        {
            var request = new RestRequest("api/todo", Method.POST);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
          
            request.AddJsonBody(new { });
            var res = client.Execute(request);
            Assert.AreEqual(401, (int)res.StatusCode);
            
        }
        [Test]
        public void TestAccessGetAllTodosWithoutToken()
        {
            var request = new RestRequest("api/todo", Method.GET);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");

            var res = client.Execute(request);
            
            Assert.AreEqual(401, (int)res.StatusCode);
        }
        [Test]
        public void TestAccessSingleTodoWithoutToken()
        {
            var request = new RestRequest("api/todo/" + Guid.NewGuid(), Method.GET);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");

            var res = client.Execute(request);
           
            Assert.AreEqual(401, (int)res.StatusCode);
        }
        [Test]
        public void TestDeleteTodoWithoutToken()
        {
            var request = new RestRequest("api/todo/" + Guid.NewGuid(), Method.DELETE);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");

            var res = client.Execute(request);

            Assert.AreEqual(401, (int)res.StatusCode);
        }
        [Test]
        public void TestUpdateTodoWithoutToken()
        {
            var request = new RestRequest("api/todo/" + Guid.NewGuid(), Method.PATCH);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");

            request.AddJsonBody(new { });
            var res = client.Execute(request);
            Assert.AreEqual(401, (int)res.StatusCode);
        }
        [Test]
        public void TestUpdateWithoutBody()
        {
            var request = new RestRequest("api/todo/" + Guid.NewGuid(), Method.PATCH);
            request.AddParameter("Content-Type", "application.json");
            request.AddHeader("accept", "text/plain");
            

            request.AddJsonBody(new { });
            request.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var res = client.Execute(request);
            var obj = JsonConvert.DeserializeObject<ToDo>(res.Content);
            
            Assert.AreEqual(500, (int)res.StatusCode);
            Assert.AreEqual("Value cannot be null.\nParameter name: entity", obj.Message);

        }
        [Test]
        public void TestUpdateValidTodo()
        {
            /**
             * Create a new Todo
             */
            var request = new RestRequest("api/todo", Method.POST);
            request.AddParameter("Content-Type", "application/json");
            request.AddHeader("accept", "text/plain");


            request.AddJsonBody(new { user = user.UserName, title = "This is a test", body = "The description of the todo", Checked = false });
            request.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var res = client.Execute(request);

            /**
             * Make sure we got it posted
             */
            var obj = JsonConvert.DeserializeObject<ToDo>(res.Content);
            Assert.AreEqual(200, (int)res.StatusCode);
            Assert.AreEqual("This is a test", obj.title);
            Assert.AreEqual("The description of the todo", obj.body);
            Assert.IsFalse(obj.Checked);

            /**
             * Update the checked to be true
             */
            string updatedTitle = "This is the updated test";
            string updatedDesc = "The description if the updated todo";

            var reqUpdateTodo = new RestRequest("api/todo/" + obj.uid, Method.PATCH);
            reqUpdateTodo.AddJsonBody(new { user = user.UserName, title = updatedTitle, body = updatedDesc, Checked = true });
            reqUpdateTodo.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var resUpdate = client.Execute(reqUpdateTodo);
          
           
            Assert.AreEqual(200, (int)res.StatusCode);

            /**
             * Get the updated Todo to verify
             */
            var reqGetTodo = new RestRequest("api/todo/" + obj.uid, Method.GET);
            reqGetTodo.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var resGetTodo = client.Execute(reqGetTodo);
            var objUpdated = JsonConvert.DeserializeObject<ToDo>(resGetTodo.Content);

            System.Console.WriteLine(resGetTodo.Content);
            Assert.AreEqual(200, (int)res.StatusCode);
            Assert.AreEqual(updatedTitle, objUpdated.title);
            Assert.AreEqual(updatedDesc, objUpdated.body);
            Assert.IsTrue(objUpdated.Checked);

            /**
             * Clean it up
             */

            var reqDelete = new RestRequest("api/todo/" + obj.uid, Method.DELETE);
            reqDelete.AddHeader("Authorization", "Bearer " + userDetails.Token);

            var resDelete = client.Execute(reqDelete);
            Assert.AreEqual(200, (int)resDelete.StatusCode);

        }
       
    }
}
