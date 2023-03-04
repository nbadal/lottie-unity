using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lottie.Model
{
    public class Bezier
    {
        [JsonProperty("c")] public bool? Closed { get; set; }
        
        [JsonProperty("i")] [JsonRequired] public List<double[]> InTangents { get; set; }
        
        [JsonProperty("o")] [JsonRequired] public List<double[]> OutTangents { get; set; }
        
        [JsonProperty("v")] [JsonRequired] public List<double[]> Vertices { get; set; }
    }

    public class Keyframe
    {
        
    }

    public class PositionKeyframe
    {
    }

    public class ShapeKeyframe
    {
    }
}