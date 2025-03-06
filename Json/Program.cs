using Json.Components;
using Json.Components.Service.Authen;
using Json.Components.Service.Authentication;
using Json.Components.Service.Data;
using Json.Components.Service.Search;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Nest;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<JsonFileService>();
builder.Services.AddSingleton<DocxToPdfService>();
builder.Services.AddSingleton<FileService>();
builder.Services.AddMudServices();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<UserAccountService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRadzenComponents();

// Configure Elasticsearch
var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
  .BasicAuthentication("elastic", "h*hsMCCEjbKK_3V_JpYS") // Replace with actual username/password
    .ServerCertificateValidationCallback((sender, certificate, chain, errors) => true) // Ignore SSL errors
    .DisableDirectStreaming() // Capture request/response in debug logs
    .DefaultIndex("skills")
    .PrettyJson();  // For readable JSON responses

var elasticClient = new ElasticClient(settings);
builder.Services.AddSingleton<IElasticClient>(elasticClient);
builder.Services.AddSingleton<SkillService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Run();
