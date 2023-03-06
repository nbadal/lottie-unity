using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lottie.Model
{
    [JsonConverter(typeof(LayerConverter))]
    public partial class Layer
    {
    }

    public class LayerConverter : SubtypeJsonConverter<Layer>
    {
        protected override Type GetTypeFromToken(JToken type) => type.ToObject<LayerType>() switch
        {
            LayerType.PreComp => typeof(PrecompositionLayer),
            LayerType.Solid => typeof(SolidColorLayer),
            LayerType.Image => typeof(ImageLayer),
            LayerType.Null => typeof(NullLayer),
            LayerType.Shape => typeof(ShapeLayer),
            LayerType.Text => typeof(TextLayer),
            LayerType.Audio => typeof(AudioLayer),
            LayerType.VideoPlaceholder => typeof(VideoPlaceholderLayer),
            LayerType.ImageSequence => typeof(ImageSequenceLayer),
            LayerType.Video => typeof(VideoLayer),
            LayerType.ImagePlaceholder => typeof(ImagePlaceholderLayer),
            LayerType.Guide => typeof(GuideLayer),
            LayerType.Adjustment => typeof(AdjustmentLayer),
            LayerType.Camera => typeof(CameraLayer),
            LayerType.Light => typeof(LightLayer),
            LayerType.Data => typeof(DataLayer),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public partial class Layer
    {
        [JsonProperty("mn")] public string MatchName;

        [JsonProperty("nm")] public string Name;
        
        [JsonProperty("ty")] [JsonRequired] public LayerType Type;

        [JsonProperty("ddd")] public int? ThreeDimensional; // TODO: IntBools

        [JsonProperty("hd")] public bool? Hidden;

        [JsonProperty("ind")] public int? Index;

        [JsonProperty("parent")] public int? Parent;

        [JsonProperty("sr")] public double? TimeStretch;

        [JsonProperty("ip")] [JsonRequired] public double InPoint;

        [JsonProperty("op")] [JsonRequired] public double OutPoint;

        [JsonProperty("st")] [JsonRequired] public double StartTime;
    }

    public class VisualLayer : Layer
    {
        [JsonProperty("cp")] public bool? CollapseTransform;

        [JsonProperty("ks")] [JsonRequired] public Transform Transform;

        [JsonProperty("ao")] public int? AutoOrient; // TODO: IntBools

        [JsonProperty("tt")] public MatteMode MatteMode;

        [JsonProperty("td")] public int? MatteTarget;

        [JsonProperty("hasMask")] public bool? HasMask;

        [JsonProperty("masksProperties")] public List<Mask> Masks;

        [JsonProperty("ef")] public List<Effect> Effects;

        [JsonProperty("mb")] public bool? MotionBlur;

        [JsonProperty("sy")] public List<Style> LayerStyle;

        [JsonProperty("bm")] public BlendMode BlendMode;

        [JsonProperty("cl")] public string CssClass;

        [JsonProperty("ln")] public string LayerXmlId;

        [JsonProperty("tg")] public string LayerXmlTagName;
    }

    public class PrecompositionLayer : VisualLayer
    {
        [JsonProperty("refId")] [JsonRequired] public string RefId;

        [JsonProperty("w")] [JsonRequired] public int Width;

        [JsonProperty("h")] [JsonRequired] public int Height;

        [JsonProperty("tm")] public AnimatedValue TimeRemapping;
    }

    public class SolidColorLayer : VisualLayer
    {
        [JsonProperty("sc")] [JsonRequired] public string Color;
        [JsonProperty("sh")] [JsonRequired] public double Height;
        [JsonProperty("sw")] [JsonRequired] public double Width;
    }

    public class ImageLayer : VisualLayer
    {
        [JsonProperty("refId")] [JsonRequired] public string RefId;
    }

    public class NullLayer : VisualLayer
    {
    }

    public class ShapeLayer : VisualLayer
    {
        [JsonProperty("shapes")]
        [JsonRequired]
        public List<Shape> Shapes;
    }

    public class TextLayer : VisualLayer
    {
        [JsonProperty("t")] [JsonRequired] public TextData Data;
    }

    public class AudioLayer : Layer
    {
        [JsonProperty("refId")] public string RefId;

        [JsonProperty("audio")] public AudioSettings AudioSettings;
    }

    public class AudioSettings
    {
        [JsonProperty("lv")] [JsonRequired] public AnimatedVector Level;
    }

    public class VideoPlaceholderLayer : Layer
    {
    }

    public class ImageSequenceLayer : Layer
    {
    }

    public class VideoLayer : Layer
    {
    }

    public class ImagePlaceholderLayer : Layer
    {
    }

    public class GuideLayer : Layer
    {
    }

    public class AdjustmentLayer : Layer
    {
    }

    public class CameraLayer : Layer
    {
        [JsonProperty("ks")] [JsonRequired] public Transform Transform;

        [JsonProperty("pe")] [JsonRequired] public AnimatedValue Perspective;
    }

    public class LightLayer : Layer
    {
    }

    public class DataLayer : Layer
    {
        [JsonProperty("refId")] public string RefId;
    }

    public class Mask
    {
        [JsonProperty("nm")] public string Name;

        [JsonProperty("mn")] public string MatchName;

        [JsonProperty("inv")] public bool? Inverted;

        [JsonProperty("pt")] public AnimatedBezier Shape;

        [JsonProperty("o")] public AnimatedValue Opacity;

        [JsonProperty("mode")] public char Mode;

        [JsonProperty("x")] public AnimatedValue Expand;
    }
}