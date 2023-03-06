using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lottie.Model
{
    public class CharacterData
    {
        [JsonProperty("ch")]
        [JsonRequired]
        public string Character;
        
        [JsonProperty("fFamily")]
        [JsonRequired]
        public string FontFamily;
        
        [JsonProperty("size")]
        [JsonRequired]
        public double FontSize;
        
        [JsonProperty("style")]
        [JsonRequired]
        public string FontStyle;
        
        [JsonProperty("w")]
        [JsonRequired]
        public double Width;
        
        [JsonProperty("data")]
        [JsonRequired]
        public CharacterDataType Data;
    }
    
    public class CharacterDataType
    {
    }

    public class CharacterShapes : CharacterDataType
    {
        [JsonProperty("shapes")]
        [JsonRequired]
        public List<Shape> Shapes;
    }

    public class CharacterPrecomp : CharacterDataType
    {
        [JsonProperty("refId")]
        [JsonRequired]
        public string RefId;
        
        [JsonProperty("ks")]
        public Transform Transform;
        
        [JsonProperty("ip")]
        public double? InPoint;
        
        [JsonProperty("op")]
        public double? OutPoint;
        
        [JsonProperty("sr")]
        public double? TimeStretch;
        
        [JsonProperty("st")]
        public double? StartTime;
    }
}