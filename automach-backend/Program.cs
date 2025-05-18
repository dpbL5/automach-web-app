using Microsoft.EntityFrameworkCore;

using automach_backend.Data;
using automach_backend.Interfaces;
using automach_backend.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddControllers();
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

