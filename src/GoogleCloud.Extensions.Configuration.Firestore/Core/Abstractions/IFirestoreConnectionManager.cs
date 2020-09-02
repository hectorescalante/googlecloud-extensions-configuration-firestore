using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GoogleCloud.Extensions.Configuration.Firestore.Core.Abstractions
{
  public interface IFirestoreConnectionManager
  {
    void Setup();
    Task<bool> IsDocumentEmptyAsync(string documentPath);
    Task SaveAsync(string documentPath, Dictionary<string, object> fields);
    Task<Dictionary<string, object>> GetDocumentFieldsAsync(ConfigurationLevels level);
    void CreateListeners(Func<ConfigurationLevels, string, Task> LoadOnChangeAsyncCallback);
    IEnumerable<ConfigurationLevels> GetConfigurationDocumentLevels();
  }
}
