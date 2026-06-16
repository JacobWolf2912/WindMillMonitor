using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mqtt.Controllers;
using System.Text;
using WindTurbineMonitor.Api.Data;
using WindTurbineMonitor.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add Entity Framework with SQLite (dev) or SQL Server (production)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add authentication services
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "your-super-secret-key-change-this-in-production";
var key = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "WindTurbineMonitor",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "WindTurbineMonitorUsers",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add MQTT Controllers and services
builder.Services.AddMqttControllers();
builder.Services.AddSingleton<AlertEvaluationService>();

// Add REST Controllers
builder.Services.AddControllers();


var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    try
    {
        db.Database.Migrate();
        app.Logger.LogInformation("Database migrated successfully");

        // Seed initial data (turbines and test user)
        await DatabaseInitializer.SeedInitialDataAsync(db, app.Logger, authService);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Failed to initialize database");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Map REST controllers
app.MapControllers();

// Fallback for SPA routing - serve index.html for non-API routes
app.MapFallback(async context =>
{
    if (!context.Request.Path.StartsWithSegments("/api"))
    {
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        var filePath = Path.Combine(env.WebRootPath, "index.html");
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(filePath);
    }
});

// Connect to MQTT broker before starting hosted services
var mqttHost = app.Configuration["Mqtt:Host"] ?? "broker.hivemq.com";
var mqttPort = int.Parse(app.Configuration["Mqtt:Port"] ?? "1883");

var mqtt = app.Services.GetService<IMqttClientService>();
if (mqtt != null)
{
    try
    {
        var connectTask = mqtt.ConnectAsync(mqttHost, mqttPort);
        if (await Task.WhenAny(connectTask, Task.Delay(10000)) == connectTask)
        {
            await connectTask;
            app.Logger.LogInformation("Connected to MQTT broker at {Host}:{Port}", mqttHost, mqttPort);
        }
        else
        {
            app.Logger.LogWarning("MQTT connection timeout - hosted service will retry on startup");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning("Failed to connect to MQTT broker: {Error}", ex.Message);
    }
}
else
{
    app.Logger.LogWarning("MQTT service not registered");
}

await app.RunAsync();
