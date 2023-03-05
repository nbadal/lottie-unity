using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lottie.Model
{
    public class CharacterData
    {
        [JsonProperty("ch")]
        [JsonRequired]
        public string Character { get; set; }
        
        [JsonProperty("fFamily")]
        [JsonRequired]
        public string FontFamily { get; set; }
        
        [JsonProperty("size")]
        [JsonRequired]
        public double FontSize { get; set; }
        
        [JsonProperty("style")]
        [JsonRequired]
        public string FontStyle { get; set; }
        
        [JsonProperty("w")]
        [JsonRequired]
        public double Width { get; set; }
        
        [JsonProperty("data")]
        [JsonRequired]
        public CharacterDataType Data { get; set; }
    }
    
    public class CharacterDataType
    {
    }

    public class CharacterShapes : CharacterDataType
    {
        [JsonProperty("shapes")]
        [JsonRequired]
        public List<Shape> Shapes { get; set; }
    }

    public class CharacterPrecomp : CharacterDataType
    {
        [JsonProperty("refId")]
        [JsonRequired]
        public string RefId { get; set; }
        
        [JsonProperty("ks")]
        public Transform Transform { get; set; }
        
        [JsonProperty("ip")]
        public double? InPoint { get; set; }
        
        [JsonProperty("op")]
        public double? OutPoint { get; set; }
        
        [JsonProperty("sr")]
        public double? TimeStretch { get; set; }
        
        [JsonProperty("st")]
        public double? StartTime { get; set; }
    }
}