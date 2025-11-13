using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Data;
using Ticklette.Domain.Models;
using Ticklette.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<TickletteContext>()
.AddDefaultTokenProviders();

// Configurar políticas de autorización
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OrganizerOnly", policy => policy.RequireClaim("customRole", "1"))
    .AddPolicy("AttendeeOnly", policy => policy.RequireClaim("customRole", "0"));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<OrganizingHouseService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<TicketTypeService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<EntryService>();
builder.Services.AddScoped<VirtualCurrencyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();