using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Data;
using Ticklette.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// DbContext
string defaultConnection = string.Empty;

if (builder.Environment.IsDevelopment())
{
    defaultConnection = builder.Configuration.GetConnectionString("DevelopmentDefaultConnection") 
                        ?? throw new InvalidOperationException("Connection string 'DevelopmentDefaultConnection' not found.");
}
else
{
    defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection") 
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<TickletteContext>(options =>
    options.UseSqlServer(defaultConnection));

// Identity
builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<TickletteContext>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Identity API endpoints
app.MapIdentityApi<User>();

app.UseAuthorization();
app.MapControllers();

app.Run();