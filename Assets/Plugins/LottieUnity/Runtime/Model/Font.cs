using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lottie.Model
{
    public class FontList
    {
        [JsonProperty("list")]
        public List<Font> List;
    }

    public class Font
    {
        [JsonProperty("ascent")]
        public double Ascent;

        [JsonProperty("fFamily")]
        [JsonRequired]
        public string FontFamily;

        [JsonProperty("fName")]
        [JsonRequired]
        public string Name;

        [JsonProperty("fStyle")]
        [JsonRequired]
        public string FStyle;
        
        [JsonProperty("fPath")]
        public string Path;
        
        [JsonProperty("fWeight")]
        public string Weight;
        
        [JsonProperty("origin")]
        public FontPathOrigin? Origin;
        
        [JsonProperty("fClass")]
        public string CssClass;
    }
    
    public enum FontPathOrigin
    {
        Local=0,
        CssUrl=1,
        ScriptUrl=2,
        FontUrl=3,
    }
}