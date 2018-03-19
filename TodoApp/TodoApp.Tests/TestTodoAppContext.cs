using System.Data.Entity;
using TodoApp.Models;

namespace TodoApp.Tests
{
    public class TestTodoAppContext : ITodoAppContext
    {
        public TestTodoAppContext()
        {
            this.Items = new TestTodoDbSet();
        }

        public DbSet<TodoItem> Items { get; set; }

        public int SaveChanges()
        {
            return 0;
        }

        public void MarkAsModified(TodoItem item) { }

        public void Dispose() { }
    }
}
