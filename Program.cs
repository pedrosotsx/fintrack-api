using ControleFinanceiro.Services;
using ControleFinanceiroApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.OpenApi.Models;

string BuildPostgresConnectionString(string databaseUrl)
{
    if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
        return databaseUrl;

    var userInfo = uri.UserInfo.Split(':', 2);
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
    var databaseName = uri.AbsolutePath.Trim('/');

    return $"Host={uri.Host};Port={uri.Port};Database={databaseName};Username={username};Password={password};Ssl Mode=Require;";
}

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = builder.Configuration["DATABASE_URL"];

if (string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(databaseUrl))
{
    connectionString = BuildPostgresConnectionString(databaseUrl);
}

builder.Services.AddDbContext<FinanceiroDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(context.Exception.Message);
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var corsOrigins = allowedOrigins
    .Concat(new[] { "https://finsecurity.vercel.app", "http://localhost:3000", "http://localhost:5173" })
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Digite: Bearer {seu_token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinanceiroDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("ReactPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();