using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lottie.Model
{
    public class FontList
    {
        [JsonProperty("list")]
        public List<Font> List { get; set; }
    }

    public class Font
    {
        [JsonProperty("ascent")]
        public double Ascent { get; set; }

        [JsonProperty("fFamily")]
        [JsonRequired]
        public string FontFamily { get; set; }

        [JsonProperty("fName")]
        [JsonRequired]
        public string Name { get; set; }

        [JsonProperty("fStyle")]
        [JsonRequired]
        public string FStyle { get; set; }
        
        [JsonProperty("fPath")]
        public string Path { get; set; }
        
        [JsonProperty("fWeight")]
        public string Weight { get; set; }
        
        [JsonProperty("origin")]
        public FontPathOrigin? Origin { get; set; }
        
        [JsonProperty("fClass")]
        public string CssClass { get; set; }
    }
    
    public enum FontPathOrigin
    {
        Local=0,
        CssUrl=1,
        ScriptUrl=2,
        FontUrl=3,
    }
}