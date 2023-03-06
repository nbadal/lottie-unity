using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lottie.Model
{
    public class Asset
    {
        [JsonProperty("id")] [JsonRequired] public string Id;

        [JsonProperty("nm")] public string Name;
    }

    public class FileAsset : Asset
    {
        [JsonProperty("u")] [JsonRequired] public string Path;

        [JsonProperty("p")] public string FileName;

        [JsonProperty("e")] public bool? Embedded; // TODO: IntBools
    }

    public class ImageAsset : FileAsset
    {
        [JsonProperty("w")] public long? Width;

        [JsonProperty("h")] public long? Height;

        [JsonProperty("t")] public string Type;
    }

    public class PrecompositionAsset : Asset
    {
        [JsonProperty("layers")][JsonRequired] public List<Layer> Layers;
    
        [JsonProperty("fr")] public double? FrameRate;
    
        [JsonProperty("xt")] public bool? Extra; // TODO: IntBools
    }

    public class SoundAsset : FileAsset
    {
    }

    public class DataSourceAsset : FileAsset
    {
        [JsonProperty("t")] public int Type;
    }
}