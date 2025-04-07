using Microsoft.EntityFrameworkCore;
using _.Data;
using _.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure SQLite connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=trials.db";
builder.Services.AddDbContext<TrialsContext>(options =>
    options.UseSqlite(connectionString));

// Register HttpClient and ClinicalTrialsService (leave as transient or scoped)
builder.Services.AddHttpClient<ClinicalTrialsService>();
// No need to register ClinicalTrialsService again explicitly if using AddHttpClient

// Register the background service using the factory approach
builder.Services.AddHostedService<ClinicalTrialsBackgroundService>();

// Configure CORS (adjust origins as needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Add controllers and Swagger for API documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinical Trials API V1");
    });
}

app.UseCors("AllowAngular");
app.MapControllers();
app.Run();
