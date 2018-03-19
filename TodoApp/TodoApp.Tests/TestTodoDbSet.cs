using System;
using System.Linq;
using TodoApp.Models;

namespace TodoApp.Tests
{
    class TestTodoDbSet : TestDbSet<TodoItem>
    {
        public override TodoItem Find(params object[] keyValues)
        {
            return this.SingleOrDefault(item => item.Id == (int)keyValues.Single());
        }
    }
}