namespace GrpcServer.Models;

public class ToDoItem
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public Status ToDoStatus { get; set; } = Status.New;
}
