using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mosparo.ApiClient
{
    public class FormValueConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(IFormValue))
            {
                return true;
            } else
            {
                return false;
            }
        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {
            return new FormValueConverter();
        }
        private class FormValueConverter : JsonConverter<object>
        {
            public override object Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                return JsonDocument.ParseValue(ref reader).RootElement.Clone();
            }

            public override void Write(
                Utf8JsonWriter writer,
                object objectToWrite,
                JsonSerializerOptions options)
            {
                if (objectToWrite is NullFormValue)
                {
                    JsonSerializer.Serialize(writer, (objectToWrite as NullFormValue).getValue(), typeof(Nullable), options);
                }
                else
                {
                    JsonSerializer.Serialize(writer, (objectToWrite as IFormValue).getValue(), (objectToWrite as IFormValue).getValue().GetType(), options);
                }
            }
        }
    }
}
