using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fora.Infrastructure.Api.SEC.Converters
{


    public class IntegerConverter : JsonConverter<int>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(string) == typeToConvert || typeof(int) == typeToConvert;
        }
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.TryGetInt64(out long l) ?
                    Convert.ToInt32(l.ToString()) :
                    reader.GetInt32();
            }

            return Convert.ToInt32(reader.GetString());

        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
