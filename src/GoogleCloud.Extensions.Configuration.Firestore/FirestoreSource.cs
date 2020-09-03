using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;

namespace GoogleCloud.Extensions.Configuration.Firestore
{
  public class FirestoreSource : JsonStreamConfigurationSource
  {
    private readonly ILogger _logger;

    public FirestoreSource(ILogger logger) : base() =>
      _logger = logger;

    public override IConfigurationProvider Build(IConfigurationBuilder builder) =>
      new FirestoreProvider(this, _logger);
  }
}
