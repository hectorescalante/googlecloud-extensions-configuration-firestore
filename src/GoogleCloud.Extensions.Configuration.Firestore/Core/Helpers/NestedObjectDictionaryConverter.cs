using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("GoogleCloud.Extensions.Configuration.Firestore.Tests")]
namespace GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers
{
  internal class NestedObjectDictionaryConverter : JsonConverter<Dictionary<string, object>>
  {
    public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType != JsonTokenType.StartObject)
        throw new JsonException("Token's type must be StartObject!");

      return (Dictionary<string, object>)ReadValue(ref reader);
    }

    private object ReadValue(ref Utf8JsonReader reader)
    {
      switch (reader.TokenType)
      {
        case JsonTokenType.StartObject:
          return ReadObject(ref reader);
        case JsonTokenType.StartArray:
          return ReadArray(ref reader);
        case JsonTokenType.String:
          return reader.GetString();
        case JsonTokenType.Number:
          if (reader.TryGetInt64(out var value))
            return value;
          else
            return reader.GetDouble();
        case JsonTokenType.True:
        case JsonTokenType.False:
          return reader.GetBoolean();
        case JsonTokenType.Null:
          return null;
        default:
          return default;
      }
    }

    private Dictionary<string, object> ReadObject(ref Utf8JsonReader reader)
    {
      var dictionary = new Dictionary<string, object>();

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
          return dictionary;

        if (reader.TokenType != JsonTokenType.PropertyName)
          throw new JsonException("Token's type must be PropertyName!");

        string propertyName = reader.GetString();

        reader.Read();
        var value = ReadValue(ref reader);

        dictionary.Add(propertyName, value);
      }

      throw new JsonException("Missing EndObject token!");
    }

    private List<object> ReadArray(ref Utf8JsonReader reader)
    {
      var list = new List<object>();

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndArray)
          return list;

        list.Add(ReadValue(ref reader));
      }

      throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
    {
      WriteObject(writer, value);
    }

    private void WriteValue(Utf8JsonWriter writer, Dictionary<string, object> value)
    {
      foreach (KeyValuePair<string, object> kvp in value)
      {
        writer.WritePropertyName(kvp.Key.ToString());

        if (kvp.Value.GetType() == typeof(Dictionary<string, object>))
        {
          WriteObject(writer, (Dictionary<string, object>)kvp.Value);
        }
        else if (kvp.Value.GetType() == typeof(List<object>))
        {
          WriteArray(writer, (List<object>)kvp.Value);
        }
        else if (kvp.Value.GetType() == typeof(string))
        {
          writer.WriteStringValue(kvp.Value.ToString());
        }
        else if (kvp.Value.GetType() == typeof(int) || kvp.Value.GetType() == typeof(Int16) || kvp.Value.GetType() == typeof(Int32) || kvp.Value.GetType() == typeof(Int64))
        {
          writer.WriteNumberValue(int.Parse(kvp.Value.ToString()));
        }
        else if (kvp.Value.GetType() == typeof(double))
        {
          writer.WriteNumberValue(double.Parse(kvp.Value.ToString()));
        }
        else if (kvp.Value.GetType() == typeof(bool))
        {
          writer.WriteBooleanValue(bool.Parse(kvp.Value.ToString()));
        }
        else
        {
          writer.WriteNullValue();
        }
      }
    }

    private void WriteObject(Utf8JsonWriter writer, Dictionary<string, object> value)
    {
      writer.WriteStartObject();

      WriteValue(writer, value);

      writer.WriteEndObject();
    }

    private void WriteArray(Utf8JsonWriter writer, List<object> value)
    {
      writer.WriteStartArray();
      foreach (Dictionary<string, object> dictionary in value)
      {
        WriteObject(writer, dictionary);
      }
      writer.WriteEndArray();
    }
  }
}
