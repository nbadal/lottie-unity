using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lottie.Model
{
    [JsonConverter(typeof(ColorConverter))]
    public class Color
    {
        public double R;
        public double G;
        public double B;
        public double? A;
    }

    class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = new Color();
            var token = JToken.Load(reader);
            if (token.Type != JTokenType.Array) throw new JsonSerializationException("Expected array");
            var array = token.ToObject<double[]>();
            if (array.Length < 3 || array.Length > 4) throw new JsonSerializationException("Expected 3 or 4 values");
            
            value.R = array[0];
            value.G = array[1];
            value.B = array[2];
            value.A = array.Length > 3 ? array[3] : 1;

            return value;
        }
    }
}