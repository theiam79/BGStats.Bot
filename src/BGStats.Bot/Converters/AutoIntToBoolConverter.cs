using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BGStats.Bot.Converters
{
  public class AutoIntToBoolConverter : JsonConverter<bool>
  {
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType == JsonTokenType.True)
      {
        return true;
      }

      if (reader.TokenType == JsonTokenType.False)
      {
        return false;
      }

      if (reader.TryGetInt32(out int value))
      {
        return value == 1;
      }

      throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
      throw new NotImplementedException("Not in scope.");
    }
  }
}
