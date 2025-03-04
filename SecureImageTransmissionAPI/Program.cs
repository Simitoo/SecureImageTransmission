using SecureImageTransmissionAPI.Hubs;
using SecureImageTransmissionAPI.Interfaces;
using SecureImageTransmissionAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors();

builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IImageGenerationService, ImageGenerationService>();
builder.Services.AddHostedService(provider => (ImageGenerationService)provider.GetRequiredService<IImageGenerationService>());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin());

app.MapHub<ImageHub>("imagehub");
app.MapHub<NotificationHub>("notifications");

app.Run();
