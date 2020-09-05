using GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GoogleCloud.Extensions.Configuration.Firestore
{
  public static class FirestoreExtensions
  {
    public static IConfigurationBuilder AddFirestoreConfiguration(this IConfigurationBuilder configurationBuilder)
    {
      var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
      return configurationBuilder.Add(new FirestoreSource(loggerFactory.CreateLogger("FirestoreConfiguration")));
    }
    public static IConfigurationBuilder AddFirestoreConfiguration(this IConfigurationBuilder configurationBuilder, ILogger logger)
    {
      return configurationBuilder.Add(new FirestoreSource(logger));
    }

    public static IConfiguration WaitForFirestoreLoad(this IConfiguration configuration, int maxWaitTime = 3000) =>
      configuration.WaitForCompleteLoad(maxWaitTime);
  }
}