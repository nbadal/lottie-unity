using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Lottie.Model
{
    public class Bezier
    {
        [JsonProperty("c")] public bool? Closed;
        
        [JsonProperty("i")] [JsonRequired] public List<double[]> InTangents;
        
        [JsonProperty("o")] [JsonRequired] public List<double[]> OutTangents;
        
        [JsonProperty("v")] [JsonRequired] public List<double[]> Vertices;

        public int SegmentCount => (Closed.HasValue && Closed.Value) ? Vertices.Count : Vertices.Count - 1;
        
        public Tuple<double[], double[], double[], double[]> Segment(int n)
        {
            var vStart = n % Vertices.Count;
            var vEnd = (n + 1) % Vertices.Count;
            var startPos = Vertices[vStart];
            var endPos = Vertices[vEnd];
            var outPos = new[] {startPos[0] + OutTangents[vStart][0], startPos[1] + OutTangents[vStart][1]};
            var inPos = new[] {endPos[0] + InTangents[vEnd][0], endPos[1] + InTangents[vEnd][1]};
            return new Tuple<double[], double[], double[], double[]>(startPos, outPos, inPos, endPos);
        }
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