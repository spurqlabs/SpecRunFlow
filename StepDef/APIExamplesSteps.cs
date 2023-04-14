using Newtonsoft.Json;
using RestSharp;
using SpurFlow.Models;
using SpurFlow.Utils;
using System;
using System.Net;
using System.Threading;
using TechTalk.SpecFlow;
using Xunit;

namespace SpurFlow.StepDef
{
    [Binding]
    public class APIExamplesSteps
    {
        private IRestResponse Response;
        private ToDoItem toDoItem;
        private APIUtil apiUtil = new APIUtil(CommonUtil.getAppConfig("api_URL"));


        [When(@"I send request to create a new ToDo item with the following information:")]
        public void WhenICreateANewToDoItemWithTheFollowingInformation(Table table)
        {
           
             var request = new RestRequest("/TodoItems", Method.POST);
             request.AddHeader("Content-Type", "application/json");
             ToDoItem toDoItem = new ToDoItem();
             toDoItem.Name = table.Rows[0]["name"];
             toDoItem.IsComplete = bool.Parse(table.Rows[0]["isComplete"]);
             request.AddJsonBody(JsonConvert.SerializeObject(toDoItem));
             IRestResponse response = apiUtil.Execute(request);
             Response = response;
        }

        [Then(@"I get the response back with status code '(.*)'")]
        public void ThenIGetTheResponseBackWithStatusCode(string StatusCode)
        {
            HttpStatusCode statusCode = Response.StatusCode;
            int numericStatusCode = (int)statusCode;
            Assert.Equal(StatusCode, Response.StatusCode.ToString());
            Assert.Equal(201, numericStatusCode);
        }

        [Given(@"I have created a ToDo Item with following information:")]
        public void GivenIHaveCreatedAToDoItemWithFollowingInformation(Table table)
        {
            var Request = new RestRequest("/TodoItems",Method.POST);
            Request.AddParameter("Content-Type", "application/json");
            var ToDoItem = new
            {
                name = table.Rows[0]["name"],
                isComplete = bool.Parse(table.Rows[0]["isComplete"])
            };
            Request.AddJsonBody(ToDoItem);
            IRestResponse<ToDoItem> Response = apiUtil.Execute<ToDoItem>(Request);
            toDoItem = Response.Data;
        }

        [When(@"I send a request to update a ToDo item with the following information:")]
        public void WhenISendARequestToUpdateAToDoItemWithTheFollowingInformation(Table table)
        {
            var Request = new RestRequest("/TodoItems/" + toDoItem.Id.ToString(), Method.PUT);
            Request.AddParameter("Content-Type", "application/json");
            toDoItem.Name = table.Rows[0]["name"];
            toDoItem.IsComplete = bool.Parse(table.Rows[0]["isComplete"]);
            Request.AddJsonBody(toDoItem);
            IRestResponse response = apiUtil.Execute(Request);
            Response = response;

        }

        [Then(@"I see the response with status code '(.*)'")]
        public void ThenISeeTheResponseWithStatusCode(int StatusCode)
        {
            HttpStatusCode statusCode = Response.StatusCode;
            int numericStatusCode = (int)statusCode;
            Assert.Equal(StatusCode, numericStatusCode);
        }

        [When(@"I send request to view all the ToDo Item")]
        public void WhenISendRequestToViewAllTheToDoItem()
        {
            var request = new RestRequest("/TodoItems", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = apiUtil.Execute(request);
            Response = response;
        }

    }
}
