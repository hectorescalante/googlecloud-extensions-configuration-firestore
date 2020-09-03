using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleCloud.Extensions.Configuration.Firestore.Core.Abstractions
{
  public interface ISecretsConnectionManager
  {
    void CreateClient();
    Task<List<KeyValuePair<string, string>>> ResolveSecretsAsync(IDictionary<string, string> settings, string defaultProjectId);
    Task<string> GetSecretValueAsync(string projectId, string secretId, string secretVersion);
  }
}
