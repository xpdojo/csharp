using Microsoft.EntityFrameworkCore;
using SimpleWebServer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(
    opt =>
        opt.UseInMemoryDatabase("TodoList")
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var todoItems = app.MapGroup("/todoitems");

// curl localhost:5274/todoitems
todoItems.MapGet("/", async (TodoDb db) =>
    await db.Todos.ToListAsync());

// curl --location 'localhost:5274/todoitems' \
// --header 'Content-Type: application/json' \
// --data '{
//     "name": "reading books"
// }'
todoItems.MapPost("/", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.Run();