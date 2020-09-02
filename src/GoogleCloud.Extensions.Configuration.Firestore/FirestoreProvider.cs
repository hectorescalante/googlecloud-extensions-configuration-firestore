using GoogleCloud.Extensions.Configuration.Firestore.Core;
using GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers;
using GoogleCloud.Extensions.Configuration.Firestore.Infrastructure;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

[assembly: InternalsVisibleTo("GoogleCloud.Extensions.Configuration.Firestore.Tests")]
namespace GoogleCloud.Extensions.Configuration.Firestore
{
  internal class FirestoreProvider : JsonStreamConfigurationProvider
  {
    private readonly ILogger _logger;
    private ApplicationSettingsManager _applicationSettings;
    private readonly FirestoreOptions _configurationOptions;
    private static readonly Mutex _mutex = new Mutex();

    public FirestoreProvider(FirestoreSource source, ILogger logger) : base(source)
    {
      _logger = logger; 
      _configurationOptions = new FirestoreOptions(logger);
    }

    public override void Load()
    {
      if (_configurationOptions.Enabled)
      {
        _logger.LogDebug($"Loading remote configuration... {DateTime.Now}");
        _applicationSettings = new ApplicationSettingsManager(_logger, _configurationOptions, new FirestoreConnectionManager(_logger, _configurationOptions), new FileManager());
        _applicationSettings.Setup().Wait();
        _applicationSettings.CreateListeners(JsonSettingsToDictionary, ReloadSettings);
      }
      else
      {
        _logger.LogWarning("Remote configuration is disabled!");
      }
    }

    public IDictionary<string, string> JsonSettingsToDictionary(string jsonSettings)
    {
      try
      {
        _logger.LogDebug($"Loading {jsonSettings}");
        Load(new MemoryStream(Encoding.UTF8.GetBytes(jsonSettings)));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw ex;
      }

      return Data;
    }

    public void ReloadSettings(ConcurrentDictionary<string, string> remoteSettingsData)
    {
      _mutex.WaitOne();
      
      //Assign the previous collected keys from all levels to the final Data dictionary.
      foreach (var item in remoteSettingsData)
      {
        if (Data.ContainsKey(item.Key)) { Data[item.Key] = item.Value; } else { Data.Add(item); };
      }

      //Add flag to indicate that load is complete.
      if (!Data.ContainsKey(LoadAwaiter.LoadStatus.Key)) { Data.Add(LoadAwaiter.LoadStatus); };

      _mutex.ReleaseMutex();

      //Refresh change token.
      _logger.LogDebug("Refreshing token...");
      OnReload();
    }
  }
}