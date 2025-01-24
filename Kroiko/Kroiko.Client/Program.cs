using Kroiko.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddKroikoServices();
builder.Services.AddRadzenComponents();

builder.Configuration.AddUserSecrets(typeof(Program).Assembly, false, true);

await builder.Build().RunAsync();