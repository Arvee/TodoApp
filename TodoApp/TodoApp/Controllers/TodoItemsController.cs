using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using TodoApp.Models;
using static System.Net.HttpStatusCode;

namespace TodoApp.Controllers
{
    public class TodoItemsController : ApiController
    {
        public ITodoAppContext db = new TodoAppContext();
        
        public TodoItemsController() { }

        public TodoItemsController(ITodoAppContext context)
        {
            db = context;
        }
        
        // GET: api/TodoItems
        [HttpGet]
        [Route("api/TodoItems")]
        public HttpResponseMessage GetTodoItems()
        {
            return Request.CreateResponse(JsonConvert.SerializeObject(GetTodoItemsList()));
        }

        public IEnumerable<TodoItem> GetTodoItemsList()
        {
            return db.Items;
        }

        public async Task<IEnumerable<TodoItem>> GetAllTodoItemsAsync()
        {
            return await Task.FromResult(GetTodoItemsList());
        }

        // GET: api/TodoItems/5
        [HttpGet]
        [Route("api/TodoItems/{id}")]
        [ResponseType(typeof(TodoItem))]
        public HttpResponseMessage GetTodoItem(int id)
        {
            TodoItem todoItem = db.Items.Find(id);
            if (todoItem == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(JsonConvert.SerializeObject(todoItem));
        }

        // PUT: api/TodoItems/5
        [HttpPut]
        [Route("api/TodoItems/{id}")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage PutTodoItem(int id, TodoItem todoItem)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (id != todoItem.Id)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            //db.Entry(todoItem).State = EntityState.Modified;
            db.MarkAsModified(todoItem);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                else
                {
                    throw ;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        //PATCH: api/TodoItems/patchItem
        [HttpPatch]
        [Route("api/TodoItems/{patchItem}")]
        public HttpResponseMessage PatchTodoItem(int itemId, TodoItemPatch patchItem)
        {
            if (patchItem == null || !ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            TodoItem todoItem = db.Items.FirstOrDefault(i => i.Id == itemId);
            if (todoItem == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            todoItem.Title = patchItem.Title;
            todoItem.Description = patchItem.Description;
            todoItem.DueDate = patchItem.DueDate;

            return Request.CreateResponse(JsonConvert.SerializeObject(todoItem));
        }

        // POST: api/TodoItems
        [HttpPost]
        [Route("api/TodoItems")]
        [ResponseType(typeof(TodoItem))]
        public HttpResponseMessage PostTodoItem([FromBody]TodoItem todoItem)
        {
            if (todoItem == null || !ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            
             db.Items.Add(todoItem);
             db.SaveChanges();
            var response = Request.CreateResponse(HttpStatusCode.Created, JsonConvert.SerializeObject(todoItem));
            string uri = Url.Link("DefaultApi", new {id = todoItem.Id});
            response.Headers.Location =  new Uri(uri);
            return response;
        }

        // DELETE: api/TodoItems/5
        [HttpDelete]
        [Route("api/TodoItems/{id}")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage DeleteTodoItem(int id)
        {
            TodoItem todoItem = db.Items.Find(id);
            if (todoItem == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Items.Remove(todoItem);
            db.SaveChanges();

            return Request.CreateResponse(JsonConvert.SerializeObject(todoItem));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TodoItemExists(int id)
        {
            return db.Items.Count(item  => item.Id == id) > 0;
        }
    }
}