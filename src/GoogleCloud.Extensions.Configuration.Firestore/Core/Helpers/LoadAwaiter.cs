using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers
{
  internal static class LoadAwaiter
  {
    public static KeyValuePair<string, string> LoadStatus { get; set; } = new KeyValuePair<string, string>("FirestoreProviderStatus", "Done");
    public static IConfiguration WaitForCompleteLoad(this IConfiguration configuration, int maxWaitTime)
    {
      var waitTime = 100;
      var currentWaitTime = 0;
      while (configuration.GetValue<string>(LoadStatus.Key) != LoadStatus.Value)
      {
        Task.Delay(waitTime).Wait();
        currentWaitTime += waitTime;
        if (currentWaitTime >= maxWaitTime)
          break;
      }
      return configuration;
    }
  }
}
