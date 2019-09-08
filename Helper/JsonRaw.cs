using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Helper
{
    public class JsonRawConverter : JsonConverter<JsonRaw>
    {
        public override void WriteJson(JsonWriter writer, JsonRaw value, JsonSerializer serializer)
        {
            writer.WriteRawValue(value.Value);
        }

        public override JsonRaw ReadJson(JsonReader reader, Type objectType, JsonRaw existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new JsonReaderException("Deserialization of JsonRaw is not supported.");
        }
    }

    [JsonConverter(typeof(JsonRawConverter))]
    public class JsonRaw
    {
        public JsonRaw(string rawString)
        {
            Value = rawString;
        }

        public string Value { get; }

        public override string? ToString()
        {
            // Should throw an exception?
            Trace.TraceWarning("ToString on JsonRaw shouldn't be called, there is somewhere an implicit ToString() happening (maybe from a manual JSON serialization).");
            return Value;
        }
    }
}
