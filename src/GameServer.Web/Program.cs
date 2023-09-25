using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FastEndpoints.Swagger.Swashbuckle;
using GameServer.Core;
using GameServer.Infrastructure;
using GameServer.Infrastructure.Data;
using GameServer.Web;
using GameServer.Web.Hubs;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;

Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
  ContentRootPath = AppContext.BaseDirectory, WebRootPath = $"wwwroot",
});

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((_, config) => {
  config.ReadFrom.Configuration(builder.Configuration);
 });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.CheckConsentNeeded = context => true;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddSingleton<ConnectionMapping<string>>();
builder.Services.AddSingleton<GroupMapping<string>>();

//string? connectionString = builder.Configuration.GetConnectionString("SqliteConnection");  //Configuration.GetConnectionString("DefaultConnection");
var connectionString = $"Data Source = {AppContext.BaseDirectory}{Path.DirectorySeparatorChar}database.sqlite";

builder.Services.AddDbContext(builder.Configuration, connectionString!);

builder.Services.AddAutoMapper(cfg =>
{
  cfg.AddProfile<DTOsProfile>();
});

builder.Services.ConfigureApplicationCookie(options =>
{
  options.Cookie.HttpOnly = true;
  options.Events.OnRedirectToLogin = context =>
  {
    context.Response.StatusCode = 401;
    return Task.CompletedTask;
  };
});

builder.Services.AddSignalR(options =>
{
  options.EnableDetailedErrors = true;
});
//   .AddMessagePackProtocol(options =>
// {
//   options.SerializerOptions = MessagePackSerializerOptions.Standard
//     .WithCompression(MessagePackCompression.None)
//     .WithSecurity(MessagePackSecurity.UntrustedData);
// });
var CorsName = "AutomationCors";
builder.Services.AddCors(option =>
{
  option.AddPolicy(CorsName, builder =>
  {
    builder.AllowAnyMethod()
      .WithMethods("DELETE")
      .AllowAnyHeader()
      .SetIsOriginAllowed(x=>true)
      .AllowCredentials();
  });
});
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
  // options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
  // options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
});
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Game Server API", Version = "v1" });
  c.EnableAnnotations();
  c.OperationFilter<FastEndpointsOperationFilter>();
});
builder.Services.AddResponseCompression(options =>
{
  options.EnableForHttps = true;
});
// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
  config.Services = new List<ServiceDescriptor>(builder.Services);

  // optional - default path to view services is /listallservices - recommended to choose your own path
  config.Path = "/listservices";
});


builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
  containerBuilder.RegisterModule(new DefaultCoreModule());
  containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});

var app = builder.Build();

app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("enforceHttps"))
{
  app.UseHttpsRedirection();
}

if (true)//app.Environment.IsDevelopment()) ///////////////////////////////////////////////////////////////////////////////////////
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware();
  app.UseWebAssemblyDebugging();
}
// else
// {
//   app.UseExceptionHandler("/Error");
//   app.UseHsts();
// }


app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
  ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseRouting();
app.UseCors(CorsName);
app.UseAuthentication();
app.UseAuthorization();

// check Swagger authentication
app.Use(async (context, next) =>
{
  var path = context.Request.Path;
  if (path!.Value!.Contains("/swagger/", StringComparison.OrdinalIgnoreCase) == true)
  {
    if (!context!.User!.Identity!.IsAuthenticated)
    {
      //context.Response.StatusCode = 401;
      //await context.Response.WriteAsync("Unauthorized");
      context.Response.Redirect("/login");
      return;
    }
  }

  await next();
});

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
app.UseResponseCompression();
//app.MapDefaultControllerRoute();
app.MapRazorPages();
app.MapControllers();

app.MapHub<ChatHub>(ChatHub.HubUrl).RequireCors(CorsName).RequireAuthorization();
app.MapHub<GameHub>(GameHub.HubUrl).RequireCors(CorsName).RequireAuthorization();
app.MapHub<MenuHub>(MenuHub.HubUrl).RequireCors(CorsName).RequireAuthorization();
app.MapFallbackToFile("index.html");

// Seed Database
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;

  // try
  // {
    await using var context = services.GetRequiredService<AppDbContext>();
    //                    context.Database.Migrate();
    context.Database.EnsureCreated();
    
    await SeedData.Initialize(services);
  // }
  // catch (Exception ex)
  // {
  //   var logger = services.GetRequiredService<ILogger<Program>>();
  //   logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
  // }
}

try
{

  app.Run();
}
catch (Exception ex)
{
  app.Logger.LogError(ex, ex.Message);
}

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program
{
}
