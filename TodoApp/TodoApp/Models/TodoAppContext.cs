using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TodoApp.Models;

namespace TodoApp.Models
{
    public class TodoAppContext : DbContext, ITodoAppContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public TodoAppContext() : base("name=TodoAppContext")
        {
        }

        public System.Data.Entity.DbSet<TodoApp.Models.TodoItem> Items { get; set; }
        //public DbSet<TodoItem> Items { get; }
        public void MarkAsModified(TodoItem item)
        {
            Entry(item).State = EntityState.Modified;
        }
    }
}
