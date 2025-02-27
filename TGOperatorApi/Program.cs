using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.ReDoc;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using TgOperator.Shared.MongoDB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TgOperator API",
        Version = "v1",
        Description = "API Documentation for TgOperator Service",
    });
    
    // Include XML comments if you have them
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add configuration
builder.Services.AddAppConfiguration(builder.Configuration);

// Add MongoDB Factory as Singleton
builder.Services.AddSingleton<IMongoDbFactory, MongoDbFactory>();

// Register PlayerService
builder.Services.AddSingleton<IPlayerService, PlayerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    
    
    // Add ReDoc UI at /api-docs
    app.UseReDoc(c =>
    {
        c.DocumentTitle = "TgOperator API Documentation";
        c.SpecUrl = "/swagger/v1/swagger.json";
        c.RoutePrefix = "api-docs";        
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();


