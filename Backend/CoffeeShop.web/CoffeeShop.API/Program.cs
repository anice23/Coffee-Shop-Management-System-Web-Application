using CoffeeShop.API.Services;
using CoffeSystem_Business;
using CoffeSystem_DataAccess;
using CoffeSystem_DataAccess.DBContext;
using CoffeSystem_DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();

    });
});

// 
var jwtKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Read JWT from the HttpOnly cookie
                if (string.IsNullOrEmpty(
                    context.Request.Headers.Authorization))
                {
                    context.Token =
                        context.Request.Cookies["access_token"];
                }

                return Task.CompletedTask;
            }
        };

    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";

    options.Cookie.Name = "__Host-CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.Path = "/";
});


// 
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductService>();

//
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<CategoryService>();

//
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
//
builder.Services.AddScoped<JwtTokenService>();


// Dashboard
builder.Services.AddScoped<DashboardRepository>();
builder.Services.AddScoped<DashboardService>();
// Cafe Settings
builder.Services.AddScoped<CafeSettingsRepository>();
builder.Services.AddScoped<CafeSettingsService>();
// Reports
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddScoped<ReportService>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT Bearer token."

    });
    options.AddSecurityDefinition("CsrfToken",
    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "X-CSRF-TOKEN",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "CSRF request token from GET /api/Auth/csrf"
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
    options.OperationFilter<CoffeeShop.API.Swagger.CsrfHeaderOperationFilter>();

});

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CoffeeShop API v1");
        
    });
}


app.UseHttpsRedirection();

app.UseCors("AngularFrontend");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
