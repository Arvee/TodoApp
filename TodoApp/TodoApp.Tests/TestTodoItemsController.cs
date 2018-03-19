using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoApp.Controllers;
using TodoApp.Models;

namespace TodoApp.Tests
{
    [TestClass]
    public class TestTodoItemsController
    {
        private TodoItem ValidTodoItem()
        {
            return new TodoItem ()
            {   Id = 1,
                Title = "Complete coding challenge",
                Description = "Impletment a web api for todo list",
                DueDate = DateTime.Today.AddDays(1)
            };
        }

        private List<TodoItem> GenSampleTodoItemsList()
        {
            List<TodoItem> items = new List<TodoItem>
            {
                new TodoItem
                {
                    Id = 1,
                    Title = "Start coding challenge",
                    Description = "Impletment a web api for todo list",
                    DueDate = DateTime.Today.AddDays(1)
                },
                new TodoItem
                {
                    Id = 2,
                    Title = "Write unit tests",
                    Description = "Unit test using the Microsoft Unit Test Framework ",
                    DueDate = DateTime.Today.AddDays(1)
                },
                new TodoItem
                {
                    Id = 3,
                    Title = "Get groceries",
                    Description = "Milk, Eggs and chicken",
                    DueDate = DateTime.Today.AddDays(2)
                },
                new TodoItem
                {
                    Id = 4,
                    Title = "Make cooffee",
                    Description = "1tbsp sugar and no cream",
                    DueDate = DateTime.Today
                },
                new TodoItem
                {
                    Id = 5,
                    Title = "Take a break",
                    Description = "Sipping coffee and scrolling the inter webs",
                    DueDate = DateTime.Today
                },
                new TodoItem
                {
                    Id = 6,
                    Title = "Call your sister",
                    Description = "No, Try not to. Avoid drama ! ",
                    DueDate = DateTime.Today.AddDays(5)
                },
                new TodoItem
                {
                    Id = 7,
                    Title = "Complete coding challenge",
                    Description = "Git commit and push to remote.",
                    DueDate = DateTime.Today.AddDays(1)
                }

            };

            return items;
        }

        //Use case 1 : GET returns a list of todo items
        [TestMethod]
        public void GetTodoItemsList_ReturnAllTodoListItems()
        {
            var ctx = new TestTodoAppContext();
            var todoList = GenSampleTodoItemsList();
            foreach (var item in todoList)
            {
                ctx.Items.Add(item);
            }
            var controller = new TodoItemsController(ctx)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var response = controller.GetTodoItems();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessStatusCode);
            TestTodoDbSet items;
            Assert.IsTrue(response.TryGetContentValue<TestTodoDbSet> (out items));
            Assert.AreEqual(7, items.Local.Count);
        }

        /*Use case 2 : GET returns a single todo item,
        *              GET returns a HTTP 404 status for a todo item that doesn't exist   
        */

        [TestMethod]
        public void GetTodoItem_ReturnItemWithSameId()
        {
            var ctx = new TestTodoAppContext();
            var sampleItem = ValidTodoItem();
            ctx.Items.Add(sampleItem);
            var controller = new TodoItemsController(ctx)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var response = controller.GetTodoItem(1); //as OkNegotiatedContentResult<TodoItem>;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessStatusCode);
            TodoItem item;
            Assert.IsTrue(response.TryGetContentValue<TodoItem>(out item));
            Assert.AreEqual(1, item.Id);
        }

        [TestMethod]
        public void GetTodoItem_Return404()
        {
            var ctx = new TestTodoAppContext();
            var sampleItem = ValidTodoItem();
            ctx.Items.Add(sampleItem);
            var controller = new TodoItemsController(ctx)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var response = controller.GetTodoItem(2); //as OkNegotiatedContentResult<TodoItem>;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        /* Use case 3: PATCH to existing item updates Title, Description and/or Due date
         *             PATCH to non existing item returns a HTTP 404 status
         */

        [TestMethod]
        public void PatchTodoItem_ReturnsUpdatedItem()
        {
            var ctx = new TestTodoAppContext();
            var controller = new TodoItemsController(ctx)
            {
                Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/TodoItems")
                },
                Configuration = new HttpConfiguration()
            };
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "TodoItems" } });

            var sampleItem = ValidTodoItem();
            ctx.Items.Add(sampleItem);
            var patchItem = new TodoItemPatch()
            {
                Title = "Pick the car form the dealership",
                Description = "Car is in the workshop and will be out from service",
                DueDate = DateTime.Now.AddDays(3)
            };
            var response = controller.PatchTodoItem(sampleItem.Id, patchItem);
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            TodoItem resultItem;
            Assert.IsTrue(response.TryGetContentValue<TodoItem>(out resultItem));
            Assert.AreEqual(sampleItem.Id, resultItem.Id);
            Assert.AreEqual(patchItem.Description, resultItem.Description);
            Assert.AreEqual(patchItem.Title, resultItem.Title);
        }

        [TestMethod]
        public void PatchTodoItem_Returns404()
        {
            var ctx = new TestTodoAppContext();
            var controller = new TodoItemsController(ctx)
            {
                Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/TodoItems")
                },
                Configuration = new HttpConfiguration()
            };
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "TodoItems" } });

            var sampleItem = ValidTodoItem();
            //sampleItem not added to the context
            var patchItem = new TodoItemPatch()
            {
                Title = "Pick the car form the dealership",
                Description = "Car is in the workshop and will be out from service",
                DueDate = DateTime.Now.AddDays(3)
            };
            var response = controller.PatchTodoItem(sampleItem.Id, patchItem);
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        } 
                 
        /*
         *Use Case 4 : POST returns a HTTP 201 status
         *             POST returns the contents of the created object
         *             Unsuccessful POST returns a HTTP 400 status
         */
        [TestMethod]
        public void PostTodoItem_Return201AndContents()
        {
            var controller = new TodoItemsController(new TestTodoAppContext())
            {
                Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/TodoItems")
                },
                Configuration = new HttpConfiguration()
            };
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );
            controller.RequestContext.RouteData  = new HttpRouteData(
                route:new HttpRoute(),
                values:new HttpRouteValueDictionary { { "controller", "TodoItems" }});

            var sampleItem = ValidTodoItem();
            var response = controller.PostTodoItem(sampleItem); //as CreatedAtRouteNegotiatedContentResult<TodoItem>;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode,HttpStatusCode.Created);
            TodoItem resultItem;
            Assert.IsTrue(response.TryGetContentValue<TodoItem>(out resultItem));
            Assert.AreEqual(sampleItem.Id, resultItem.Id);
            Assert.AreEqual(sampleItem.Description, resultItem.Description);
            Assert.AreEqual(sampleItem.Title, resultItem.Title);
        }

        [TestMethod]
        public void PostTodoItem_Return400()
        {
            var controller = new TodoItemsController(new TestTodoAppContext())
            {
                Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/TodoItems")
                },
                Configuration = new HttpConfiguration()
            };
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "TodoItems" } });

            var response = controller.PostTodoItem(null);
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }
        
    }
}
