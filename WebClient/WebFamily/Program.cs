using Blazor.FamilyTreeJS;
using Blazor.Core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebFamily;
using Blazor.FamilyTreeJS.Components.Interop.Modules.FamilyTreeStatic;
using WebFamily.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://backtree-dzagariy.amvera.io") });
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazorFamilyJS();
builder.Services.AddSingleton<FamilyNodeFactory>();
builder.Services.AddSingleton<FamilyTreeStaticModule>();
builder.Services.AddScoped<IFamilyClientService, FamilyClientService>();
var webHost = builder
    .Build()
    .ConfigureIJSRuntimeJsonOptionsForBlazorFamilyTree();

var familyTreeStaticModule = webHost.Services.GetRequiredService<FamilyTreeStaticModule>();
await familyTreeStaticModule.ImportAsync();
await familyTreeStaticModule.SetFamilyClinkCurveAsync(3.5f);

await webHost.RegisterCallbackReviverAsync();
await webHost.RunAsync();