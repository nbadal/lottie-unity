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
        [JsonProperty("mn")] public string MatchName { get; set; }

        [JsonProperty("nm")] public string Name { get; set; }
        
        [JsonProperty("ty")] [JsonRequired] public LayerType Type { get; set; }

        [JsonProperty("ddd")] public int? ThreeDimensional { get; set; } // TODO: IntBools

        [JsonProperty("hd")] public bool? Hidden { get; set; }

        [JsonProperty("ind")] public int? Index { get; set; }

        [JsonProperty("parent")] public int? Parent { get; set; }

        [JsonProperty("sr")] public double? TimeStretch { get; set; }

        [JsonProperty("ip")] [JsonRequired] public double InPoint { get; set; }

        [JsonProperty("op")] [JsonRequired] public double OutPoint { get; set; }

        [JsonProperty("st")] [JsonRequired] public double StartTime { get; set; }
    }

    public class VisualLayer : Layer
    {
        [JsonProperty("cp")] public bool? CollapseTransform { get; set; }

        [JsonProperty("ks")] [JsonRequired] public Transform Transform { get; set; }

        [JsonProperty("ao")] public int? AutoOrient { get; set; } // TODO: IntBools

        [JsonProperty("tt")] public MatteMode MatteMode { get; set; }

        [JsonProperty("td")] public int? MatteTarget { get; set; }

        [JsonProperty("hasMask")] public bool? HasMask { get; set; }

        [JsonProperty("masksProperties")] public List<Mask> Masks { get; set; }

        [JsonProperty("ef")] public List<Effect> Effects { get; set; }

        [JsonProperty("mb")] public bool? MotionBlur { get; set; }

        [JsonProperty("sy")] public List<Style> LayerStyle { get; set; }

        [JsonProperty("bm")] public BlendMode BlendMode { get; set; }

        [JsonProperty("cl")] public string CssClass { get; set; }

        [JsonProperty("ln")] public string LayerXmlId { get; set; }

        [JsonProperty("tg")] public string LayerXmlTagName { get; set; }
    }

    public class PrecompositionLayer : VisualLayer
    {
        [JsonProperty("refId")] [JsonRequired] public string RefId { get; set; }

        [JsonProperty("w")] [JsonRequired] public int Width { get; set; }

        [JsonProperty("h")] [JsonRequired] public int Height { get; set; }

        [JsonProperty("tm")] public AnimatedValue TimeRemapping { get; set; }
    }

    public class SolidColorLayer : VisualLayer
    {
        [JsonProperty("sc")] [JsonRequired] public string Color { get; set; }
        [JsonProperty("sh")] [JsonRequired] public double Height { get; set; }
        [JsonProperty("sw")] [JsonRequired] public double Width { get; set; }
    }

    public class ImageLayer : VisualLayer
    {
        [JsonProperty("refId")] [JsonRequired] public string RefId { get; set; }
    }

    public class NullLayer : VisualLayer
    {
    }

    public class ShapeLayer : VisualLayer
    {
        [JsonProperty("shapes")]
        [JsonRequired]
        public List<Shape> Shapes { get; set; }
    }

    public class TextLayer : VisualLayer
    {
        [JsonProperty("t")] [JsonRequired] public TextData Data { get; set; }
    }

    public class AudioLayer : Layer
    {
        [JsonProperty("refId")] public string RefId { get; set; }

        [JsonProperty("audio")] public AudioSettings AudioSettings { get; set; }
    }

    public class AudioSettings
    {
        [JsonProperty("lv")] [JsonRequired] public AnimatedVector Level { get; set; }
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
        [JsonProperty("ks")] [JsonRequired] public Transform Transform { get; set; }

        [JsonProperty("pe")] [JsonRequired] public AnimatedValue Perspective { get; set; }
    }

    public class LightLayer : Layer
    {
    }

    public class DataLayer : Layer
    {
        [JsonProperty("refId")] public string RefId { get; set; }
    }

    public class Mask
    {
        [JsonProperty("nm")] public string Name { get; set; }

        [JsonProperty("mn")] public string MatchName { get; set; }

        [JsonProperty("inv")] public bool? Inverted { get; set; }

        [JsonProperty("pt")] public AnimatedBezier Shape { get; set; }

        [JsonProperty("o")] public AnimatedValue Opacity { get; set; }

        [JsonProperty("mode")] public char Mode { get; set; }

        [JsonProperty("x")] public AnimatedValue Expand { get; set; }
    }
}