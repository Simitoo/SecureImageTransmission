using Microsoft.AspNetCore.Authentication.JwtBearer;
using SecureImageTransmissionAPI.Common.Options;
using SecureImageTransmissionAPI.Hubs;
using SecureImageTransmissionAPI.Interfaces;
using SecureImageTransmissionAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7020", "https://dev-uby1yrtmiyahcuyc.eu.auth0.com", "http://localhost:5080")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            /*.WithExposedHeaders("Authorization")*/;
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.Authority = "https://dev-uby1yrtmiyahcuyc.eu.auth0.com";
        options.Audience = "https://secureimageapi.com";

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/imagehub"))
                {
                    context.Token = accessToken;
                }
                else if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    var token = authHeader.ToString().Replace("Bearer ", "");
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(path: "logs/log.json", rollingInterval: RollingInterval.Day, formatter: new Serilog.Formatting.Json.JsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IImageGenerationService, ImageGenerationService>();
builder.Services.AddHostedService(provider => (ImageGenerationService)provider.GetRequiredService<IImageGenerationService>());

builder.Services.Configure<FormatOptions>(builder.Configuration.GetSection("ImageGeneration:FormatOptions"));
builder.Services.Configure<ImageSizeOptions>(builder.Configuration.GetSection("ImageGeneration:SizeOptions"));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
app.UseCors("CorsPolicy");

app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.MapHub<ImageHub>("/imagehub").RequireCors("CorsPolicy").AllowAnonymous();
app.MapHub<NotificationHub>("/notifications");

app.Run();
