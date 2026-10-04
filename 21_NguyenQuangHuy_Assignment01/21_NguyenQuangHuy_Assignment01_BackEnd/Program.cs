using System;
using System.Text;
using _21_NguyenQuangHuy_Assignment01_BackEnd.Services;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Repositories.Implementations;
using Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. OData EDM Model
var odataBuilder = new ODataConventionModelBuilder();
odataBuilder.EntitySet<NewsArticle>("NewsArticles");
odataBuilder.EntitySet<NewsArticle>("PublicNews");
odataBuilder.EntitySet<Category>("Categories");
odataBuilder.EntitySet<SystemAccount>("SystemAccounts");
odataBuilder.EntitySet<Tag>("Tags");
odataBuilder.EntityType<SystemAccount>().HasKey(a => a.AccountId);
odataBuilder.EntityType<Category>().HasKey(c => c.CategoryId);
odataBuilder.EntityType<NewsArticle>().HasKey(n => n.NewsArticleId);
odataBuilder.EntityType<Tag>().HasKey(t => t.TagId);
odataBuilder.EntityType<SystemAccount>().Ignore(a => a.AccountPassword);

builder.Services.AddControllers()
    .AddOData(opt => opt
        .Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
        .AddRouteComponents("odata", odataBuilder.GetEdmModel()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 2. JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"] ?? "ThisIsASuperSecretKeyForFUNews_PRN232_AtLeast32Chars!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "FUNewsManagement.API",
            ValidAudience = jwtSettings["Audience"] ?? "FUNewsManagement.Client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
builder.Services.AddAuthorization();

// 3. Dependency Injection
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddSingleton<JwtTokenService>();

// 4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// 5. Swagger with Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FUNewsManagement API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
