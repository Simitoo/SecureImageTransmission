using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SecureImageTransmissionClient;
using SecureImageTransmissionClient.Models;
using SecureImageTransmissionClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var authSettings = new AuthSettings();
builder.Configuration.GetSection("AuthSettings").Bind(authSettings);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(authSettings);
builder.Services.AddScoped<Auth0TokenHandler>();
builder.Services.AddScoped<SignalRService>();

builder.Services.AddHttpClient("SecureImageTransmissionAPI", client =>
{
    client.BaseAddress = new Uri(authSettings.ApiBaseUrl);
})
    .AddHttpMessageHandler<Auth0TokenHandler>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
