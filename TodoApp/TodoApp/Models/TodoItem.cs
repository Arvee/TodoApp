using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }
}