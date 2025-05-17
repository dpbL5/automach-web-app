using automach_backend.Data;
using Microsoft.EntityFrameworkCore;

using automach_backend.Interfaces;
using automach_backend.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

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
app.MapControllers();
app.Run();

