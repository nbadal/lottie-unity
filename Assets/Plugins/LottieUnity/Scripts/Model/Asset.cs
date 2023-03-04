using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lottie.Model
{
    public class Asset
    {
        [JsonProperty("id")] [JsonRequired] public string Id { get; set; }

        [JsonProperty("nm")] public string Name { get; set; }
    }

    public class FileAsset : Asset
    {
        [JsonProperty("u")] [JsonRequired] public string Path { get; set; }

        [JsonProperty("p")] public string FileName { get; set; }

        [JsonProperty("e")] public bool? Embedded { get; set; } // TODO: IntBools
    }

    public class ImageAsset : FileAsset
    {
        [JsonProperty("w")] public long? Width { get; set; }

        [JsonProperty("h")] public long? Height { get; set; }

        [JsonProperty("t")] public string Type { get; set; }
    }

    public class PrecompositionAsset : Asset
    {
        [JsonProperty("layers")][JsonRequired] public List<Layer> Layers { get; set; }
    
        [JsonProperty("fr")] public double? FrameRate { get; set; }
    
        [JsonProperty("xt")] public bool? Extra { get; set; } // TODO: IntBools
    }

    public class SoundAsset : FileAsset
    {
    }

    public class DataSourceAsset : FileAsset
    {
        [JsonProperty("t")] public int Type { get; set; }
    }
}