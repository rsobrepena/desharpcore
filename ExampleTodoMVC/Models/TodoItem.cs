using System.ComponentModel.DataAnnotations;

namespace ExampleTodoMVC.Models;

public class TodoItem
{
    public int Id { get; set; }

    [Required, StringLength(100, MinimumLength = 3)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Notes { get; set; }

    public Priority Priority { get; set; } = Priority.Normal;

    public DateTime? DueDate { get; set; }

    public bool IsDone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum Priority { Low, Normal, High }
