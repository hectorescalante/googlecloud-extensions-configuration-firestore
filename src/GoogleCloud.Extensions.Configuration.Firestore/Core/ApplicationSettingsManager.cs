using GoogleCloud.Extensions.Configuration.Firestore.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleCloud.Extensions.Configuration.Firestore.Core
{
  internal class ApplicationSettingsManager
  {
    private readonly FirestoreOptions _options;
    private readonly ILogger _logger;
    private readonly IFirestoreConnectionManager _connectionManager;
    private readonly IFileManager _fileManager;

    public ConcurrentDictionary<string, string> ConfigData { get; private set; } = new ConcurrentDictionary<string, string>();
    public Func<string, IDictionary<string, string>> JsonSettingsToDictionarySettings { get; private set; }
    public Action<ConcurrentDictionary<string, string>> ReloadSettings { get; private set; }

    public ApplicationSettingsManager(ILogger logger, FirestoreOptions options, IFirestoreConnectionManager connectionManager, IFileManager fileManager)
    {
      _logger = logger;
      _options = options;
      _connectionManager = connectionManager;
      _fileManager = fileManager;
    }

    public async Task Setup()
    {
      _logger.LogDebug($"Begin setup... {DateTime.Now}");
      _connectionManager.Setup();

      _logger.LogDebug($"Procesing AppSettings for {_options.ApplicationName}...");
      await CreateAppSettingsDocument();

      _logger.LogDebug($"Procesing StageSettings for {_options.ApplicationName}...");
      await CreateStageSettingsDocument();
      _logger.LogDebug($"End setup... {DateTime.Now}");
    }
    private async Task CreateAppSettingsDocument()
    {
      //Create application document if not exists
      if (await _connectionManager.IsDocumentEmptyAsync(_options.GetApplicationDocumentPath()))
      {
        _logger.LogDebug($"Creating application settings document from {_options.SettingsFileName}");
        var remoteSettingsDocument = new ApplicationSettingsDocument();
        remoteSettingsDocument.SetData(_fileManager.GetFileContent(_options.SettingsFileName));
        await _connectionManager.SaveAsync(_options.GetApplicationDocumentPath(), remoteSettingsDocument.Data.ToDictionary());
      }
    }
    private async Task CreateStageSettingsDocument()
    {
      //Create stage document if not exists
      if (await _connectionManager.IsDocumentEmptyAsync(_options.GetStageDocumentPath()))
      {
        _logger.LogDebug($"Creating {_options.StagesCollection} {_options.ReleaseStage}");
        await _connectionManager.SaveAsync(_options.GetStageDocumentPath(), new Dictionary<string, object>());
      }
    }

    public void CreateListeners(Func<string, IDictionary<string, string>> jsonSettingsToDictionarySettingsCallback, Action<ConcurrentDictionary<string, string>> reloadSettingsCallback)
    {
      JsonSettingsToDictionarySettings = jsonSettingsToDictionarySettingsCallback;
      ReloadSettings = reloadSettingsCallback;

      _logger.LogDebug("Creating listeners...");
      _connectionManager.CreateListeners(LoadDocumentSettingsOnChangeAsync);
    }

    public async Task LoadDocumentSettingsOnChangeAsync(ConfigurationLevels level, string snapshotId)
    {
      _logger.LogInformation($"Configuration change detected... Level: {level}, Document: {snapshotId}");
      //Remove all keys for a new load.
      ConfigData.Clear();
      //When a change is made in one level we must load all other levels in order to merge all settings ordered by relevance (application -> stage -> machine).
      foreach (var configurationLevel in _connectionManager.GetConfigurationDocumentLevels())
      {
        _logger.LogDebug($"Loading Documents by Level. Current:{configurationLevel}");
        var remoteSettingsDocument = await _connectionManager.GetDocumentFieldsAsync(configurationLevel);
        //Use this FirestoreConfigurationProvider method to convert the json settings into a dictionary with IConfiguration format.
        var dataDictionary = JsonSettingsToDictionarySettings(remoteSettingsDocument.ToJson());
        //Add settings to a centralized final dictionary.
        dataDictionary.ToList().ForEach(item => ConfigData.AddOrUpdate(item.Key.ToLower(), item.Value, (key, value) => value = item.Value));
      }
      //Use this FirestoreConfigurationProvider method in order to have access the private Data Dictionary and refresh the token.
      ReloadSettings(ConfigData);
      _logger.LogDebug($"End of detected change load! {DateTime.Now}");
    }
  }
}