using Microsoft.AspNetCore.Authentication.JwtBearer;
using SecureImageTransmissionAPI.Common.Options;
using SecureImageTransmissionAPI.Hubs;
using SecureImageTransmissionAPI.Interfaces;
using SecureImageTransmissionAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/imagehub") ||
                path.StartsWithSegments("/notifications"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:images", policy =>
    {
        policy.RequireClaim("scope", "read:images");
    });
});

builder.Services.AddSignalR();
builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.MapHub<ImageHub>("imagehub");
app.MapHub<NotificationHub>("notifications");

app.Run();
