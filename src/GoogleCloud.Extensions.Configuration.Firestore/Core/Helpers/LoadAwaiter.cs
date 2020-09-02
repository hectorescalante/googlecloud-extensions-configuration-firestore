using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers
{
  internal static class LoadAwaiter
  {
    public static KeyValuePair<string, string> LoadStatus { get; set; } = new KeyValuePair<string, string>("FirestoreProviderStatus", "Done");
    public static async Task WaitForCompleteLoad(this IConfiguration configuration)
    {
      var maxWaitTime = 5000;
      var waitTime = 1000;
      var currentWaitTime = 0;
      while (configuration.GetValue<string>(LoadStatus.Key) != LoadStatus.Value)
      {
        await Task.Delay(waitTime);
        currentWaitTime += waitTime;
        if (currentWaitTime >= maxWaitTime)
          break;
      }
    }
  }
}
