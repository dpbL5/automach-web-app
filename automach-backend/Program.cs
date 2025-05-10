using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

var todos = new List<string>();
app.MapGet("/todos", () =>
{
    return todos;
})
.WithName("GetTodos")
.WithOpenApi()
.WithTags("Todos");
app.MapPost("/todos", (string todo) =>
{
    todos.Add(todo);
    return Results.Created($"/todos/{todo}", todo);
})
.WithName("CreateTodo")
.WithOpenApi()
.WithTags("Todos");
app.MapPut("/todos/{index}", (int index, string todo) =>
{
    if (index < 0 || index >= todos.Count)
    {
        return Results.NotFound();
    }
    todos[index] = todo;
    return Results.NoContent();
})
.WithName("UpdateTodo")
.WithOpenApi()
.WithTags("Todos");

app.Run();
Todo todo = new Todo("Todo", false);



public record Todo(string Name, bool IsCompleted);





// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
