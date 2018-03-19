using System;
using System.Data.Entity;

namespace TodoApp.Models
{
    public interface ITodoAppContext :IDisposable
    {
        DbSet<TodoItem> Items { get; }
        int SaveChanges();
        void MarkAsModified(TodoItem item);
    }
}
