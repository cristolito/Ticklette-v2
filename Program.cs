using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Ticklette.Domain.Data;
using Ticklette.Domain.Models;
using Ticklette.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración para Somee.com
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50MB para uploads
});

// Add services to the container.
builder.Services.AddControllers();
// Swagger

builder.Services.AddCors(options =>
{
    // Política más permisiva para desarrollo
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
    
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ticklette API", Version = "v1" });

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// DbContext
string defaultConnection = string.Empty;

// if (builder.Environment.IsDevelopment())
// {
//     defaultConnection = builder.Configuration.GetConnectionString("DevelopmentDefaultConnection")
//                         ?? throw new InvalidOperationException("Connection string 'DevelopmentDefaultConnection' not found.");
// }
// else
// {
//     defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
//                         ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// }
defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<TickletteContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});
// .AddGoogle(options =>
// {
//     options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
//     options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
// });

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

// Cloudinary Service
builder.Services.AddScoped<CloudinaryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

app.UseHttpsRedirection();

// IMPORTANTE: Authentication antes de Authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();