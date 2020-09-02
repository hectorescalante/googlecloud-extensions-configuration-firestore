using AutoFixture;
using AutoFixture.AutoMoq;
using GoogleCloud.Extensions.Configuration.Firestore;
using GoogleCloud.Extensions.Configuration.Firestore.Core;
using GoogleCloud.Extensions.Configuration.Firestore.Core.Abstractions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Tests.UnitTests
{
  public class ApplicationSettingsManagerTests
  {
    private readonly IFixture _autoFixture;
    private readonly ITestOutputHelper _outputHelper;

    public ApplicationSettingsManagerTests(ITestOutputHelper outputHelper)
    {
      _autoFixture = new Fixture().Customize(new AutoMoqCustomization());
      _outputHelper = outputHelper;
    }

    [Fact]
    public async Task TestSetup_WithConfigEnabled_ShouldSuccess()
    {
      //Arrange
      var connectionMock = _autoFixture.Freeze<Mock<IFirestoreConnectionManager>>();
      connectionMock
        .Setup(mock => mock.IsDocumentEmptyAsync(It.IsAny<string>()))
        .ReturnsAsync(true);
      connectionMock
        .Setup(mock => mock.GetDocumentFieldsAsync(It.IsAny<ConfigurationLevels>()))
        .ReturnsAsync(new Dictionary<string, object>());

      //Act
      var sut = _autoFixture.Create<ApplicationSettingsManager>();
      await sut.Setup();

      //Assert
      connectionMock.Verify(mock => mock.Setup(), Times.Once);
      connectionMock.Verify(mock => mock.IsDocumentEmptyAsync(It.IsAny<string>()), Times.Exactly(2));
      connectionMock.Verify(mock => mock.SaveAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Exactly(2));
    }

    [Fact]
    public async Task TestLoadDocumentSettingsOnChangeAsync_WithAppDocumentOnly_ShouldSuccess()
    {
      //Arrange
      var provider = _autoFixture.Create<FirestoreProvider>();
      var appSettings = _autoFixture.Create<TestSettings>();
      var appSettingsDocument = new ApplicationSettingsDocument();
      var appSettingsJson = JsonSerializer.Serialize(appSettings);
      _outputHelper.WriteLine(appSettingsJson);
      appSettingsDocument.SetData(appSettingsJson);

      var connectionMock = _autoFixture.Freeze<Mock<IFirestoreConnectionManager>>();
      connectionMock
        .Setup(mock => mock.GetConfigurationDocumentLevels())
        .Returns(new List<ConfigurationLevels>() { { ConfigurationLevels.Application } });
      connectionMock
        .Setup(mock => mock.GetDocumentFieldsAsync(It.IsAny<ConfigurationLevels>()))
        .ReturnsAsync(appSettingsDocument.Data.ToDictionary());

      //Act
      var sut = _autoFixture.Create<ApplicationSettingsManager>();
      sut.CreateListeners(provider.JsonSettingsToDictionary, provider.ReloadSettings);
      await sut.LoadDocumentSettingsOnChangeAsync(ConfigurationLevels.Application, "snapshotId");

      //Assert
      Assert.NotEmpty(sut.ConfigData);
    }

    [Fact]
    public async Task TestLoadDocumentSettingsOnChangeAsync_WithStageDocument_ShouldSuccess()
    {
      //Arrange
      var provider = _autoFixture.Create<FirestoreProvider>();

      var appSettings = _autoFixture.Create<TestSettings>();
      var appSettingsDocument = new ApplicationSettingsDocument();
      appSettingsDocument.SetData(JsonSerializer.Serialize(appSettings));

      var stageSettings = _autoFixture.Create<TestSettings>();
      var stageSettingsDocument = new ApplicationSettingsDocument();
      stageSettingsDocument.SetData(JsonSerializer.Serialize(stageSettings));

      var connectionMock = _autoFixture.Freeze<Mock<IFirestoreConnectionManager>>();
      connectionMock
        .Setup(mock => mock.GetConfigurationDocumentLevels())
        .Returns(new List<ConfigurationLevels>() { { ConfigurationLevels.Application }, { ConfigurationLevels.Stage } });
      connectionMock
        .Setup(mock => mock.GetDocumentFieldsAsync(It.Is<ConfigurationLevels>(param => param == ConfigurationLevels.Application)))
        .ReturnsAsync(appSettingsDocument.Data.ToDictionary());
      connectionMock
        .Setup(mock => mock.GetDocumentFieldsAsync(It.Is<ConfigurationLevels>(param => param == ConfigurationLevels.Stage)))
        .ReturnsAsync(stageSettingsDocument.Data.ToDictionary());

      //Act
      var sut = _autoFixture.Create<ApplicationSettingsManager>();
      sut.CreateListeners(provider.JsonSettingsToDictionary, provider.ReloadSettings);
      await sut.LoadDocumentSettingsOnChangeAsync(ConfigurationLevels.Stage, "snapshotId");

      //Assert
      Assert.NotEmpty(sut.ConfigData);
      Assert.NotEqual(appSettings.Id, int.Parse(sut.ConfigData["id"]));
      Assert.NotEqual(appSettings.Name, sut.ConfigData["name"]);
      Assert.Equal(stageSettings.Id, int.Parse(sut.ConfigData["id"]));
      Assert.Equal(stageSettings.Name, sut.ConfigData["name"]);
    }

    [Fact]
    public async Task TestLoadDocumentSettingsOnChangeAsync_WithTagDocument_ShouldSuccess()
    {
      //Arrange
      var provider = _autoFixture.Create<FirestoreProvider>();

      var appSettings = _autoFixture.Create<TestSettings>();
      var appSettingsDocument = new ApplicationSettingsDocument();
      appSettingsDocument.SetData(JsonSerializer.Serialize(appSettings));

      var stageSettings = _autoFixture.Create<TestSettings>();
      var stageSettingsDocument = new ApplicationSettingsDocument();
      stageSettingsDocument.SetData(JsonSerializer.Serialize(stageSettings));

      var machineSettings = _autoFixture.Create<TestSettings>();
      var machineSettingsDocument = new ApplicationSettingsDocument();
      machineSettingsDocument.SetData(JsonSerializer.Serialize(machineSettings));

      var tagSettings = _autoFixture.Create<TestSettings>();
      var tagSettingsDocument = new ApplicationSettingsDocument();
      tagSettingsDocument.SetData(JsonSerializer.Serialize(tagSettings));

      var connectionMock = _autoFixture.Freeze<Mock<IFirestoreConnectionManager>>();
      connectionMock
        .Setup(mock => mock.GetConfigurationDocumentLevels())
        .Returns(new List<ConfigurationLevels>() { { ConfigurationLevels.Application }, { ConfigurationLevels.Stage }, { ConfigurationLevels.Tag } });
      connectionMock
        .Setup(mock => mock.GetDocumentFieldsAsync(It.Is<ConfigurationLevels>(param => param == ConfigurationLevels.Application)))
        .ReturnsAsync(appSettingsDocument.Data.ToDictionary());
      connectionMock
        .Setup(mock => mock.GetDocumentFieldsAsync(It.Is<ConfigurationLevels>(param => param == ConfigurationLevels.Stage)))
        .ReturnsAsync(stageSettingsDocument.Data.ToDictionary());
      connectionMock
        .Setup(mock => mock.GetDocumentFieldsAsync(It.Is<ConfigurationLevels>(param => param == ConfigurationLevels.Tag)))
        .ReturnsAsync(tagSettingsDocument.Data.ToDictionary());

      //Act
      var sut = _autoFixture.Create<ApplicationSettingsManager>();
      sut.CreateListeners(provider.JsonSettingsToDictionary, provider.ReloadSettings);
      await sut.LoadDocumentSettingsOnChangeAsync(ConfigurationLevels.Tag, "snapshotId");

      //Assert
      Assert.NotEmpty(sut.ConfigData);
      Assert.NotEqual(appSettings.Id, int.Parse(sut.ConfigData["id"]));
      Assert.NotEqual(appSettings.Name, sut.ConfigData["name"]);
      Assert.NotEqual(stageSettings.Id, int.Parse(sut.ConfigData["id"]));
      Assert.NotEqual(stageSettings.Name, sut.ConfigData["name"]);
      Assert.NotEqual(machineSettings.Id, int.Parse(sut.ConfigData["id"]));
      Assert.NotEqual(machineSettings.Name, sut.ConfigData["name"]);
      Assert.Equal(tagSettings.Id, int.Parse(sut.ConfigData["id"]));
      Assert.Equal(tagSettings.Name, sut.ConfigData["name"]);
    }

  }
}
