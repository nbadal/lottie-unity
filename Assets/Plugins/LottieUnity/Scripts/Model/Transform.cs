using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lottie.Model
{
    [JsonConverter(typeof(TransformConverter))]
    public class Transform
    {
        [JsonProperty("a")] public AnimatedPosition AnchorPoint { get; set; }
        
        [JsonProperty("s")] public AnimatedVector Scale { get; set; }
        
        [JsonProperty("o")] public AnimatedValue Opacity { get; set; }
        
        [JsonProperty("sk")] public AnimatedValue Skew { get; set; }
        
        [JsonProperty("sa")] public AnimatedValue SkewAxis { get; set; }
        
        [JsonProperty("p")] public ITransformPosition Position { get; set; }

        internal bool IsAnimated()
        {
            return Scale?.IsAnimated == 1
                   || Skew?.IsAnimated == 1
                   || SkewAxis?.IsAnimated == 1
                   || (this is AngleRotationTransform art && art.Rotation?.IsAnimated == 1)
                   || (this is SplitRotationTransform srt && (
                       srt.RotationX?.IsAnimated == 1
                       || srt.RotationY?.IsAnimated == 1
                       || srt.RotationZ?.IsAnimated == 1
                   ))
                   || (AnchorPoint?.IsAnimated() ?? false)
                   || (Position?.IsAnimated() ?? false);
        }
    }

    public class AngleRotationTransform : Transform
    {
        [JsonProperty("r")] public AnimatedValue Rotation { get; set; }
    }
    
    public class SplitRotationTransform : Transform
    {
        [JsonProperty("rx")] public AnimatedValue RotationX { get; set; }
        
        [JsonProperty("ry")] public AnimatedValue RotationY { get; set; }
        
        [JsonProperty("rz")] public AnimatedValue RotationZ { get; set; }
        
        [JsonProperty("or")] public AnimatedVector Orientation { get; set; }
    }
    
    [JsonConverter(typeof(TransformPositionConverter))]
    public interface ITransformPosition
    {
        public bool IsAnimated();
    }

    public class TransformConverter : SubtypeJsonConverter<Transform>
    {
        protected override JToken GetTypeToken(JToken token) => token;

        protected override Type GetTypeFromToken(JToken type)
        {
            if (type["r"] != null)
            {
                return typeof(AngleRotationTransform);
            }
            if (type["rx"] != null || type["ry"] != null || type["rz"] != null)
            {
                return typeof(SplitRotationTransform);
            }
            return typeof(Transform);
        }
    }

    public class TransformPositionConverter : JsonConverter<ITransformPosition>
    {
        public override void WriteJson(JsonWriter writer, ITransformPosition value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;

        public override ITransformPosition ReadJson(JsonReader reader, Type objectType, ITransformPosition existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token["x"] != null || token["y"] != null || token["z"] != null)
            {
                return token.ToObject<AnimatedSplitVector>();
            }
            return token.ToObject<AnimatedPosition>();
        }
    }
}