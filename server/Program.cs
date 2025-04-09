using System.Text.Json.Serialization;
using ClinicalTrialMatcher.Data;
using ClinicalTrialMatcher.Interfaces;
using ClinicalTrialMatcher.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Read the connection string from configuration.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Host=localhost;Database=clinicaltrials;Username=student;Password=Kaleab66488605!";

// Register the TrialsContext with PostgreSQL and enable vector support.
builder.Services.AddDbContext<TrialsContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.UseVector();
    }));

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod());

app.MapControllers();

app.Run();
