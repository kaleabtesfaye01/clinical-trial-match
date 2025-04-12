using System.Text.Json.Serialization;
using ClinicalTrialMatcher.Data;
using ClinicalTrialMatcher.Extensions;
using ClinicalTrialMatcher.Interfaces;
using ClinicalTrialMatcher.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Read the connection string from configuration.
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"] ??
    builder.Configuration["DB_CONNECTION_STRING"];
var openAiKey = builder.Configuration["OpenAI:ApiKey"] ??
    builder.Configuration["OPENAI_API_KEY"];

// Register the TrialsContext with PostgreSQL and enable vector support.
builder.Services.AddDbContext<TrialsContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.UseVector();
    }));

// Register the OpenAI API key in the configuration.
builder.Services.AddHttpClient<IVectorizationService>(client =>
{
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openAiKey);
});

// Register HttpClient and our service implementations.
builder.Services.AddHttpClient<IClinicalTrialsService, ClinicalTrialsService>();
builder.Services.AddHttpClient<IVectorizationService, VectorizationService>();

// Register our matching service and vectorization service.
builder.Services.AddScoped<ITrialMatchingService, TrialMatchingService>();
builder.Services.AddScoped<IVectorizationService, VectorizationService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
    app.UseVectorization();
}

app.MapControllers();

app.Run();
