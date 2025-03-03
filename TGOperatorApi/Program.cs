using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.ReDoc;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using TgOperator.Shared.MongoDB;
using TgOperator.Features.Players;
using TgOperator.Features.Players.PostProcessors;
using Sentry;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add Sentry
builder.WebHost.UseSentry(options =>
{
    var sentrySettings = builder.Configuration
        .GetSection(nameof(SentrySettings))
        .Get<SentrySettings>();
    
    options.Dsn = sentrySettings?.Dsn;
    options.Debug = sentrySettings?.Debug ?? false;
    options.TracesSampleRate = sentrySettings?.TracesSampleRate ?? 1.0;
    options.MinimumEventLevel = Enum.Parse<LogLevel>(sentrySettings?.MinimumEventLevel ?? "Error");
    options.SendDefaultPii = sentrySettings?.SendDefaultPii ?? true;
    options.AttachStacktrace = sentrySettings?.AttachStacktrace ?? true;
    options.Environment = sentrySettings?.Environment ?? builder.Environment.EnvironmentName;
});

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

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddRequestPostProcessor(typeof(OperatorApiLogPostProcessor<,>));
});

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


