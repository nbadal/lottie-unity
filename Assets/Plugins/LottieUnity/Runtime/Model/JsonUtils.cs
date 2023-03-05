using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Lottie.Model
{
    public abstract class Utils
    {
        public static JsonReader CopyReaderForObject(JsonReader reader, JToken jToken)
        {
            JsonReader jTokenReader = jToken.CreateReader();
            jTokenReader.Culture = reader.Culture;
            jTokenReader.DateFormatString = reader.DateFormatString;
            jTokenReader.DateParseHandling = reader.DateParseHandling;
            jTokenReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jTokenReader.FloatParseHandling = reader.FloatParseHandling;
            jTokenReader.MaxDepth = reader.MaxDepth;
            jTokenReader.SupportMultipleContent = reader.SupportMultipleContent;
            return jTokenReader;
        }
    }

    public abstract class SubtypeJsonConverter<T> : JsonConverter<T>
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var ty = GetTypeToken(token);
            if (ty == null)
            {
                throw new JsonSerializationException("Type not found");
            }

            var value = (T)Activator.CreateInstance(GetTypeFromToken(ty));
            using var subReader = Utils.CopyReaderForObject(reader, token);
            serializer.Populate(subReader, value);
            
            PostRead(value, token, serializer);

            return value;
        }
        
        protected virtual void PostRead(T value, JToken token, JsonSerializer serializer) { }

        protected virtual JToken GetTypeToken(JToken token) => token["ty"];

        protected abstract Type GetTypeFromToken(JToken type);
    }
}