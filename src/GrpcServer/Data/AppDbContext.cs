using GrpcServer.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
}