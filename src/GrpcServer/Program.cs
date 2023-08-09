using GrpcServer.Data;
using GrpcServer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string connectionString = "Server=localhost;User Id=postgres;Password=password;Port=5432;Database=Grpc;";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddGrpc().AddJsonTranscoding();

var app = builder.Build();

app.MapGrpcService<ToDoService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
