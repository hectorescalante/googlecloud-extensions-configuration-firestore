using Google.Cloud.SecretManager.V1;
using GoogleCloud.Extensions.Configuration.Firestore.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleCloud.Extensions.Configuration.Firestore.Infrastructure
{
  internal class SecretsConnectionManager : ISecretsConnectionManager
  {
    private readonly ILogger _logger;

    public SecretManagerServiceClient SecretManagerClient { get; private set; }

    public SecretsConnectionManager(ILogger logger)
    {
      _logger = logger;
    }

    public void CreateClient()
    {
      _logger.LogDebug($"Creating secret manager client ...");
      if (SecretManagerClient == null)
        SecretManagerClient = SecretManagerServiceClient.Create();
    }

    public async Task<List<KeyValuePair<string, string>>> ResolveSecretsAsync(IDictionary<string, string> settings, string defaultProjectId)
    {
      var secretSettings = new List<KeyValuePair<string, string>>();
      foreach (var setting in settings.Where(s => s.Value.StartsWith("secret:")))
      {
        CreateClient();
        var secretReference = setting.Value.Split(':');
        var projectId = secretReference.Length < 2 || string.IsNullOrEmpty(secretReference[1]) ? defaultProjectId : secretReference[1];
        var secretId = secretReference.Length < 3 || string.IsNullOrEmpty(secretReference[2]) ? null : secretReference[2];
        var secretVersion = secretReference.Length < 4 || string.IsNullOrEmpty(secretReference[3]) ? "latest" : secretReference[3];

        if (secretId != null)
        {
          var secretValue = await GetSecretValueAsync(projectId, secretId, secretVersion);
          secretSettings.Add(new KeyValuePair<string, string>(setting.Key, secretValue));
        }
      }
      return secretSettings;
    }

    public async Task<string> GetSecretValueAsync(string projectId, string secretId, string secretVersionId)
    {
      var secretName = new SecretVersionName(projectId, secretId, secretVersionId);
      var secret = await SecretManagerClient.AccessSecretVersionAsync(secretName);
      return secret.Payload.Data.ToStringUtf8();
    }
  }
}
