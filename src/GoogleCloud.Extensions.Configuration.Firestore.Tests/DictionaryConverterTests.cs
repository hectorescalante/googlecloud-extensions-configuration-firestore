using AutoFixture;
using AutoFixture.AutoMoq;
using GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace Tests.UnitTests
{
  public class DictionaryConverterTests
  {
    private readonly IFixture _autoFixture;
    private readonly ITestOutputHelper _outputHelper;

    public DictionaryConverterTests(ITestOutputHelper outputHelper)
    {
      _autoFixture = new Fixture().Customize(new AutoMoqCustomization());
      _outputHelper = outputHelper;
    }

    [Fact]
    public void TestRead_WithJsonString_ShouldReturnDictionary()
    {
      //Arrange
      var jsonSettings = _autoFixture.Create<TestSettings>();
      jsonSettings.Amount = 100.00;
      var jsonSettingsString = JsonSerializer.Serialize(jsonSettings);
      var jsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonSettingsString));
      _outputHelper.WriteLine(jsonSettingsString);

      //Act
      var sut = _autoFixture.Create<NestedObjectDictionaryConverter>();
      jsonReader.Read();
      var dictionary = sut.Read(ref jsonReader, typeof(Dictionary<string, object>), new JsonSerializerOptions());

      //Assert
      Assert.Equal(jsonSettings.Id, int.Parse(dictionary["Id"].ToString()));
      Assert.Equal(jsonSettings.Name, dictionary["Name"].ToString());
      Assert.Equal(jsonSettings.Active, bool.Parse(dictionary["Active"].ToString()));
      Assert.Equal(jsonSettings.Amount, double.Parse(dictionary["Amount"].ToString()));
      Assert.Equal(jsonSettings.Detail.Status, int.Parse(((Dictionary<string, object>)dictionary["Detail"])["Status"].ToString()));
      Assert.Equal(jsonSettings.Detail.Description, ((Dictionary<string, object>)dictionary["Detail"])["Description"].ToString());
      foreach (Dictionary<string, object> item in (List<object>)dictionary["Details"])
      {
        Assert.Contains(jsonSettings.Details, d => d.Status == int.Parse(item["Status"].ToString()));
        Assert.Contains(jsonSettings.Details, d => d.Description == item["Description"].ToString());
      }
    }

    [Fact]
    public void TestWrite_WithDictionary_ShouldReturnValidJson()
    {
      //Arrange
      var jsonString = "";
      var inputDictionary = new Dictionary<string, object>() {
        { "Id", 1 },
        { "Name", "TestName" },
        { "Active", true },
        { "Amount", 100.99},
        { "Detail", 
          new Dictionary<string, object>()
          {
            {"Status", 0 },
            {"Description", "TestDescription0" }
          }
        },
        { "Details", 
          new List<object>() {
            new Dictionary<string, object>()
            {
              {"Status", 1 },
              {"Description", "TestDescription1" }
            },
            new Dictionary<string, object>()
            {
              {"Status", 2 },
              {"Description", "TestDescription2" }
            },
            new Dictionary<string, object>()
            {
              {"Status", 3 },
              {"Description", "TestDescription3" }
            }
          }
        }
      };

      //Act
      var sut = _autoFixture.Create<NestedObjectDictionaryConverter>();
      using (var stream = new MemoryStream())
      {
        using (var jsonWriter = new Utf8JsonWriter(stream))
        {
          sut.Write(jsonWriter, inputDictionary, new JsonSerializerOptions());
        }
        jsonString = Encoding.UTF8.GetString(stream.ToArray());
        _outputHelper.WriteLine(jsonString);
      }

      //Assert
      var rootElement = JsonDocument.Parse(jsonString).RootElement;
      Assert.Equal(JsonValueKind.Object, rootElement.ValueKind);
      Assert.Equal(int.Parse(inputDictionary["Id"].ToString()), rootElement.GetProperty("Id").GetInt64());
      Assert.Equal(inputDictionary["Name"].ToString(), rootElement.GetProperty("Name").GetString());
      Assert.Equal(bool.Parse(inputDictionary["Active"].ToString()), rootElement.GetProperty("Active").GetBoolean());
      Assert.Equal(double.Parse(inputDictionary["Amount"].ToString()), rootElement.GetProperty("Amount").GetDouble());
      Assert.Equal(int.Parse(((Dictionary<string, object>)inputDictionary["Detail"])["Status"].ToString()), rootElement.GetProperty("Detail").GetProperty("Status").GetInt64());
      Assert.Equal(((Dictionary<string, object>)inputDictionary["Detail"])["Description"].ToString(), rootElement.GetProperty("Detail").GetProperty("Description").GetString());
      var index = 0;
      foreach (Dictionary<string, object> item in (List<object>)inputDictionary["Details"])
      {
        Assert.Equal(int.Parse(item["Status"].ToString()), rootElement.GetProperty("Details")[index].GetProperty("Status").GetInt64());
        Assert.Equal(item["Description"].ToString(), rootElement.GetProperty("Details")[index].GetProperty("Description").GetString());
        index++;
      }
    }

  }
}
