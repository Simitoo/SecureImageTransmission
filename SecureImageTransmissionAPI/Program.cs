using SecureImageTransmissionAPI.Common.Options;
using SecureImageTransmissionAPI.Hubs;
using SecureImageTransmissionAPI.Interfaces;
using SecureImageTransmissionAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

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
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin());

app.MapHub<ImageHub>("imagehub");
app.MapHub<NotificationHub>("notifications");

app.Run();
