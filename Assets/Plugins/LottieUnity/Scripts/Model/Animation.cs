using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lottie.Model
{
    public partial class LottieAnimation
    {
        [JsonProperty("mn")] public string MatchName { get; set; }

        [JsonProperty("nm")] public string Name { get; set; }

        [JsonProperty("v")] public string Version { get; set; }

        [JsonProperty("fr")] [JsonRequired] public double FrameRate { get; set; }

        [JsonProperty("ip")] [JsonRequired] public double InPoint { get; set; }

        [JsonProperty("op")] [JsonRequired] public double OutPoint { get; set; }

        [JsonProperty("w")] [JsonRequired] public int Width { get; set; }

        [JsonProperty("h")] [JsonRequired] public int Height { get; set; }

        [JsonProperty("ddd")] public int? ThreeDimensional { get; set; }

        [JsonProperty("assets")] public List<Asset> Assets { get; set; }

        [JsonProperty("comps")] public List<PrecompositionAsset> Comps { get; set; }

        [JsonProperty("fonts")] public FontList Fonts { get; set; }

        [JsonProperty("chars")] public List<CharacterData> Chars { get; set; }

        [JsonProperty("meta")] public Metadata Meta { get; set; }

        [JsonProperty("metadata")] public UserMetadata Metadata { get; set; }

        [JsonProperty("mb")] public MotionBlur Mb { get; set; }

        [JsonProperty("markers")] public List<Marker> Markers { get; set; }

        [JsonProperty("layers")]
        [JsonRequired]
        public List<Layer> Layers { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("a")] public string Author { get; set; }

        [JsonProperty("d")] public string Description { get; set; }

        [JsonProperty("tc")] public string ThemeColor { get; set; }

        [JsonProperty("g")] public string Generator { get; set; }

        [JsonProperty("k")] public Keywords Keywords { get; set; }
    }

    public class Keywords : List<string>
    {
        // TODO: deserializer that takes a single string rather than an array
    }

    public class UserMetadata
    {
        [JsonProperty("filename")] public string Filename { get; set; }

        [JsonProperty("customProps")] public object CustomProps { get; set; }
    }

    public class MotionBlur
    {
        [JsonProperty("sa")] public double? ShutterAngle { get; set; }

        [JsonProperty("sp")] public double? ShutterPhase { get; set; }

        [JsonProperty("spf")] public double? SamplesPerFrame { get; set; }

        [JsonProperty("asl")] public double? AdaptiveSampleLimit { get; set; }
    }

    public class Marker
    {
        [JsonProperty("cm")] public string Comment { get; set; }

        [JsonProperty("tm")] public double? Time { get; set; }

        [JsonProperty("dr")] public double? Duration { get; set; }
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