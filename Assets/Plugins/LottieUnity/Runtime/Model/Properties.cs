using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lottie.Model
{
    public class AnimatableProperty<TKeyframe, TStatic>
    {
        [JsonProperty("ix")] public int? Index { get; set; }

        [JsonProperty("a")] public int? IsAnimated { get; set; } // TODO: IntBool

        [JsonProperty("x")] public string Expression { get; set; }

        /*[JsonProperty("k")]*/
        public List<TKeyframe> Values { get; set; }

        /*[JsonProperty("k")]*/
        public TStatic Value { get; set; }
    }

    public class AnimatablePropertyConverter<TKeyframe, TStatic> : JsonConverter<AnimatableProperty<TKeyframe, TStatic>>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, AnimatableProperty<TKeyframe, TStatic> value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static AnimatableProperty<TKeyframe, TStatic> Create()
        {
            // Determine implementation from generic types
            if (typeof(TKeyframe) == typeof(Keyframe) && typeof(TStatic) == typeof(double?))
            {
                return new AnimatedValue() as AnimatableProperty<TKeyframe, TStatic>;
            }

            if (typeof(TKeyframe) == typeof(Keyframe) && typeof(TStatic) == typeof(Color))
            {
                return new AnimatedColor() as AnimatableProperty<TKeyframe, TStatic>;
            }

            if (typeof(TKeyframe) == typeof(Keyframe) && typeof(TStatic) == typeof(List<double>))
            {
                return new AnimatedVector() as AnimatableProperty<TKeyframe, TStatic>;
            }

            if (typeof(TKeyframe) == typeof(PositionKeyframe) && typeof(TStatic) == typeof(List<double>))
            {
                return new AnimatedPosition() as AnimatableProperty<TKeyframe, TStatic>;
            }

            if (typeof(TKeyframe) == typeof(ShapeKeyframe) && typeof(TStatic) == typeof(Bezier))
            {
                return new AnimatedBezier() as AnimatableProperty<TKeyframe, TStatic>;
            }

            throw new NotImplementedException("Unsupported generics: " + typeof(TKeyframe) + ", " + typeof(TStatic));
        }

        public override AnimatableProperty<TKeyframe, TStatic> ReadJson(JsonReader reader, Type objectType,
            AnimatableProperty<TKeyframe, TStatic> existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var value = Create();
            var token = JToken.Load(reader);

            using var subReader = Utils.CopyReaderForObject(reader, token);
            serializer.Populate(subReader, value);

            // Deserialize k value
            var kToken = token["k"] ?? throw new JsonSerializationException("k not found");
            switch (value.IsAnimated)
            {
                case 1:
                    value.Values = kToken.ToObject<List<TKeyframe>>();
                    break;
                case 0:
                    value.Value = kToken.ToObject<TStatic>();
                    break;
                default:
                    throw new JsonSerializationException("Invalid IsAnimated value");
            }

            return value;
        }
    }

    [JsonConverter(typeof(AnimatablePropertyConverter<Keyframe, double?>))]
    public class AnimatedValue : AnimatableProperty<Keyframe, double?>
    {
    }

    [JsonConverter(typeof(AnimatablePropertyConverter<Keyframe, Color>))]
    public class AnimatedColor : AnimatableProperty<Keyframe, Color>
    {
    }


    [JsonConverter(typeof(AnimatablePropertyConverter<Keyframe, List<double>>))]
    public class AnimatedVector : AnimatableProperty<Keyframe, List<double>>
    {
        [JsonProperty("l")] public int? Length { get; set; }
    }

    [JsonConverter(typeof(AnimatablePropertyConverter<PositionKeyframe, List<double>>))]
    public class AnimatedPosition : AnimatableProperty<PositionKeyframe, List<double>>, ITransformPosition
    {
        public new bool IsAnimated() => base.IsAnimated == 1;
    }


    [JsonConverter(typeof(AnimatablePropertyConverter<ShapeKeyframe, Bezier>))]
    public class AnimatedBezier : AnimatableProperty<ShapeKeyframe, Bezier>
    {
    }

    public class AnimatedSplitVector : ITransformPosition
    {
        public bool IsAnimated() => X.IsAnimated == 1 || Y.IsAnimated == 1 || Z.IsAnimated == 1;
        
        [JsonProperty("x")] [JsonRequired] public AnimatedValue X { get; set; }

        [JsonProperty("y")] [JsonRequired] public AnimatedValue Y { get; set; }

        [JsonProperty("z")] public AnimatedValue Z { get; set; }
    }

    public class AnimatedGradient
    {
        [JsonProperty("k")] [JsonRequired] public AnimatedVector Colors { get; set; }

        [JsonProperty("p")] public int Count { get; set; }
    }
}