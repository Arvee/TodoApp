using System;
using System.ComponentModel.DataAnnotations;


namespace TodoApp.Models
{
    public class TodoItemPatch
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}