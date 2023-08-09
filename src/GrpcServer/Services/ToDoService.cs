using Grpc.Core;
using GrpcServer.Data;
using GrpcServer.Models;
using Microsoft.EntityFrameworkCore;
using Status = Grpc.Core.Status;

namespace GrpcServer.Services;

public class ToDoService : ToDoIt.ToDoItBase
{
    private readonly AppDbContext _dbContext;

    public ToDoService(AppDbContext context)
    {
        _dbContext = context;
    }

    public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
    {
        if (request.Title == string.Empty || request.Description == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid object"));

        var toDoItem = new ToDoItem
        {
            Title = request.Title,
            Description = request.Description
        };

        await _dbContext.AddAsync(toDoItem);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new CreateToDoResponse
        {
            Id = toDoItem.Id
        });
    }

    public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var toDoItems = await _dbContext.ToDoItems.ToListAsync();

        foreach (var toDo in toDoItems)
        {
            response.ToDo.Add(new ReadToDoResponse
            {
                Id = toDo.Id,
                Title = toDo.Title,
                Description = toDo.Description,
                ToDoStatus = toDo.ToDoStatus.ToString()
            });
        }

        return await Task.FromResult(response);
    }

    public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
    {
        var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (toDoItem is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Invalid id"));

        return await Task.FromResult(new ReadToDoResponse
        {
            Id = toDoItem.Id,
            Title = toDoItem.Title,
            Description = toDoItem.Description,
            ToDoStatus = toDoItem.ToDoStatus.ToString()
        });
    }

    public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
    {
        if (request.Title == string.Empty || request.Description == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid object"));

        var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (toDoItem is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Invalid id"));

        toDoItem.Title = request.Title;
        toDoItem.Description = request.Description;
        toDoItem.ToDoStatus = (GrpcServer.Models.Status)Enum.Parse(typeof(GrpcServer.Models.Status), request.ToDoStatus);

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new UpdateToDoResponse
        {
            Id = toDoItem.Id
        });
    }

    public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
    {
        var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (toDoItem is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Invalid id"));

        var response = new DeleteToDoResponse
        {
            Id = toDoItem.Id
        };

        _dbContext.ToDoItems.Remove(toDoItem);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(response);
    }
}
