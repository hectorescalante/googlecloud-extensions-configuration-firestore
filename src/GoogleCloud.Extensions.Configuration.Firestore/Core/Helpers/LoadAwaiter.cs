using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers
{
  internal static class LoadAwaiter
  {
    public static KeyValuePair<string, string> LoadStatus { get; set; } = new KeyValuePair<string, string>("LoadStatus?", "Done!");
    public static async Task<IConfiguration> WaitForCompleteLoad(this IConfiguration configuration, int maxWaitTime)
    {
      if (new FirestoreOptions().IsEnabled())
      {
        var waitTime = 100;
        var waitTimeLimit = DateTime.UtcNow.AddMilliseconds(maxWaitTime);
        while (configuration.GetValue<string>(LoadStatus.Key) != LoadStatus.Value)
        {
          await Task.Delay(waitTime);
          if (DateTime.UtcNow.Ticks >= waitTimeLimit.Ticks)
            break;
        }
      }
      return configuration;
    }
  }
}
