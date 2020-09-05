using GoogleCloud.Extensions.Configuration.Firestore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;

namespace GoogleCloud.Extensions.Configuration.Sample
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

      try
      {
        Log.Information("Starting application...");
        CreateHostBuilder(args).Build().Run();
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Unexpected exception! Ending application...");
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
              var loggerFactory = LoggerFactory.Create(builder => { builder.AddSerilog(); });
              config.AddFirestoreConfiguration(loggerFactory.CreateLogger("FirestoreConfiguration"));
            })
            .UseSerilog((hostingContext, loggerConfiguration) => {
              hostingContext.Configuration.WaitForFirestoreLoad();
              Log.Information("Test for secret resolution: {SecretValue}", hostingContext.Configuration.GetValue<string>("Weather:Days"));
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}
