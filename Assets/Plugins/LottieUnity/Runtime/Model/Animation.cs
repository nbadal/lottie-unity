using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lottie.Model
{
    public partial class LottieAnimation
    {
        [JsonProperty("mn")] public string MatchName;

        [JsonProperty("nm")] public string Name;

        [JsonProperty("v")] public string Version;

        [JsonProperty("fr")] [JsonRequired] public double FrameRate;

        [JsonProperty("ip")] [JsonRequired] public double InPoint;

        [JsonProperty("op")] [JsonRequired] public double OutPoint;

        [JsonProperty("w")] [JsonRequired] public int Width;

        [JsonProperty("h")] [JsonRequired] public int Height;

        [JsonProperty("ddd")] public int? ThreeDimensional;

        [JsonProperty("assets")] public List<Asset> Assets;

        [JsonProperty("comps")] public List<PrecompositionAsset> Comps;

        [JsonProperty("fonts")] public FontList Fonts;

        [JsonProperty("chars")] public List<CharacterData> Chars;

        [JsonProperty("meta")] public Metadata Meta;

        [JsonProperty("metadata")] public UserMetadata Metadata;

        [JsonProperty("mb")] public MotionBlur Mb;

        [JsonProperty("markers")] public List<Marker> Markers;

        [JsonProperty("layers")] [JsonRequired]
        public List<Layer> Layers;
    }

    public class Metadata
    {
        [JsonProperty("a")] public string Author;

        [JsonProperty("d")] public string Description;

        [JsonProperty("tc")] public string ThemeColor;

        [JsonProperty("g")] public string Generator;

        [JsonProperty("k")] public Keywords Keywords;
    }

    public class Keywords : List<string>
    {
        // TODO: deserializer that takes a single string rather than an array
    }

    public class UserMetadata
    {
        [JsonProperty("filename")] public string Filename;

        [JsonProperty("customProps")] public object CustomProps;
    }

    public class MotionBlur
    {
        [JsonProperty("sa")] public double? ShutterAngle;

        [JsonProperty("sp")] public double? ShutterPhase;

        [JsonProperty("spf")] public double? SamplesPerFrame;

        [JsonProperty("asl")] public double? AdaptiveSampleLimit;
    }

    public class Marker
    {
        [JsonProperty("cm")] public string Comment;

        [JsonProperty("tm")] public double? Time;

        [JsonProperty("dr")] public double? Duration;
    }


    public partial class LottieAnimation
    {
        public static LottieAnimation FromJson(string json) => JsonConvert.DeserializeObject<LottieAnimation>(json);
    }

    public static class Serialize
    {
        public static string ToJson(this LottieAnimation self) => JsonConvert.SerializeObject(self);
    }
}