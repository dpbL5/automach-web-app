using automach_backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Swagger for API documentation
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection"))); // Use SQL Server

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

