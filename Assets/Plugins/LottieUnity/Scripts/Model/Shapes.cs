using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lottie.Model
{
    public class ShapeConverter : SubtypeJsonConverter<Shape>
    {
        protected override Type GetTypeFromToken(JToken type)
        {
            var typeString = type.ToObject<string>();
            return typeString switch
            {
                "rc" => typeof(RectangleShape),
                "el" => typeof(EllipseShape),
                "sr" => typeof(PolystarShape),
                "sh" => typeof(PathShape),
                "fl" => typeof(FillShape),
                "st" => typeof(StrokeShape),
                "gf" => typeof(GradientFillShape),
                "gs" => typeof(GradientStrokeShape),
                "no" => typeof(NoStyleShape),
                "gr" => typeof(GroupShape),
                "tr" => typeof(TransformShape),
                "rp" => typeof(RepeaterShape),
                "tm" => typeof(TrimShape),
                "rd" => typeof(RoundedCornersShape),
                "pb" => typeof(PuckerBloatShape),
                "mm" => typeof(MergeShape),
                "tw" => typeof(TwistShape),
                "op" => typeof(OffsetPathShape),
                "zz" => typeof(ZigZagShape),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override void PostRead(Shape value, JToken token, JsonSerializer serializer)
        {
            if (value is TransformShape tr)
            {
                tr.Transform = token.ToObject<Transform>(serializer);
            }
        }
    }

    public enum ShapeDirection
    {
        Normal = 1,
        Reversed = 3,
    }


    [JsonConverter(typeof(ShapeConverter))]
    public class Shape
    {
        [JsonProperty("nm")] public string Name { get; set; }

        [JsonProperty("mn")] public string MatchName { get; set; }

        [JsonProperty("hd")] public bool? Hidden { get; set; }

        [JsonProperty("ty")] [JsonRequired] public string Type { get; set; }

        [JsonProperty("bm")] public BlendMode? BlendMode { get; set; }

        [JsonProperty("ix")] public int? PropertyIndex { get; set; }

        [JsonProperty("cl")] public string CssClass { get; set; }

        [JsonProperty("ln")] public string LayerXmlId { get; set; }
    }

    public class ShapeWithDirection : Shape
    {
        [JsonProperty("d")] public ShapeDirection Direction { get; set; }
    }

    class EllipseShape : ShapeWithDirection
    {
        [JsonProperty("p")] [JsonRequired] public AnimatedPosition Position { get; set; }

        [JsonProperty("s")] [JsonRequired] public AnimatedVector Size { get; set; }
    }

    class FillShape : Shape
    {
        [JsonProperty("o")] [JsonRequired] public AnimatedValue Opacity { get; set; }

        [JsonProperty("c")] [JsonRequired] public AnimatedColor Color { get; set; }

        [JsonProperty("r")] public FillRule? FillRule { get; set; }
    }

    class GradientShape : Shape
    {
        [JsonProperty("s")] [JsonRequired] public AnimatedVector StartPoint { get; set; }

        [JsonProperty("e")] [JsonRequired] public AnimatedVector EndPoint { get; set; }

        [JsonProperty("g")] [JsonRequired] public AnimatedGradient Colors { get; set; }

        [JsonProperty("t")] public GradientType? GradientType { get; set; }

        // Radial:

        [JsonProperty("h")] public AnimatedValue HighlightLength { get; set; }

        [JsonProperty("a")] public AnimatedValue HighlightAngle { get; set; }
    }

    enum FillRule
    {
        NonZero = 1,
        EvenOdd = 2,
    }

    enum GradientType
    {
        Linear = 1,
        Radial = 2,
    }

    class GradientFillShape : GradientShape
    {
        [JsonProperty("o")] [JsonRequired] public AnimatedValue Opacity { get; set; }

        [JsonProperty("r")] public FillRule? FillRule { get; set; }
    }

    class GradientStrokeShape : GradientShape
    {
        [JsonProperty("lc")] public LineCap? LineCap { get; set; }

        [JsonProperty("lj")] public LineJoin? LineJoin { get; set; }

        [JsonProperty("ml")] public double? MiterLimit { get; set; }

        [JsonProperty("ml2")] public AnimatedValue AnimatedMiterLimit { get; set; }

        [JsonProperty("o")] [JsonRequired] public AnimatedValue Opacity { get; set; }

        [JsonProperty("w")] [JsonRequired] public AnimatedValue StrokeWidth { get; set; }

        [JsonProperty("d")] public List<StrokeDash> Dashes { get; set; }
    }

    class GroupShape : Shape
    {
        [JsonProperty("np")] public double? NumProperties { get; set; }

        [JsonProperty("it")] public List<Shape> Shapes { get; set; }

        [JsonProperty("cix")] public int? PropertyCIndex { get; set; }
    }

    class PathShape : ShapeWithDirection
    {
        [JsonProperty("ks")] [JsonRequired] public AnimatedBezier Shape { get; set; }
    }

    class PolystarShape : ShapeWithDirection
    {
        [JsonProperty("p")] [JsonRequired] public AnimatedPosition Position { get; set; }

        [JsonProperty("or")] [JsonRequired] public AnimatedValue OuterRadius { get; set; }

        [JsonProperty("os")] [JsonRequired] public AnimatedValue OuterRoundness { get; set; }

        [JsonProperty("r")] [JsonRequired] public AnimatedValue Rotation { get; set; }

        [JsonProperty("pt")] [JsonRequired] public AnimatedValue Points { get; set; }

        [JsonProperty("sy")] public StarType? StarType { get; set; }

        // TODO: required if StarType is Star:

        [JsonProperty("ir")] public AnimatedValue InnerRadius { get; set; }

        [JsonProperty("is")] public AnimatedValue InnerRoundness { get; set; }
    }

    enum StarType
    {
        Star = 1,
        Polygon = 2,
    }

    class PuckerBloatShape : Shape
    {
        [JsonProperty("a")] public AnimatedValue Amount { get; set; }
    }

    class RectangleShape : ShapeWithDirection
    {
        [JsonProperty("p")] [JsonRequired] public AnimatedPosition Position { get; set; }

        [JsonProperty("s")] [JsonRequired] public AnimatedVector Size { get; set; }

        [JsonProperty("r")] [JsonRequired] public AnimatedValue Radius { get; set; }
    }

    class ModifierShape : Shape
    {
    }

    class RepeaterShape : ModifierShape
    {
        [JsonProperty("c")] [JsonRequired] public AnimatedValue Copies { get; set; }

        [JsonProperty("o")] public AnimatedValue Offset { get; set; }

        [JsonProperty("m")] public Composite? Composite { get; set; }

        [JsonProperty("tr")] [JsonRequired] public List<RepeaterTransform> Transform { get; set; }
    }

    class RepeaterTransform
    {
        // TODO: Transform properties

        [JsonProperty("so")] public AnimatedValue StartOpacity { get; set; }

        [JsonProperty("eo")] public AnimatedValue EndOpacity { get; set; }
    }

    enum Composite
    {
        Above = 1,
        Below = 2,
    }

    class RoundedCornersShape : ModifierShape
    {
        [JsonProperty("r")] [JsonRequired] public AnimatedValue Radius { get; set; }
    }

    class StrokeShape : Shape
    {
        [JsonProperty("lc")] public LineCap? LineCap { get; set; }

        [JsonProperty("lj")] public LineJoin? LineJoin { get; set; }

        [JsonProperty("ml")] public double? MiterLimit { get; set; }

        [JsonProperty("ml2")] public AnimatedValue AnimatedMiterLimit { get; set; }

        [JsonProperty("o")] [JsonRequired] public AnimatedValue Opacity { get; set; }

        [JsonProperty("w")] [JsonRequired] public AnimatedValue StrokeWidth { get; set; }

        [JsonProperty("d")] public List<StrokeDash> Dashes { get; set; }

        [JsonProperty("c")] [JsonRequired] public AnimatedColor Color { get; set; }
    }

    class StrokeDash
    {
        [JsonProperty("nm")] public string Name { get; set; }

        [JsonProperty("mn")] public string MatchName { get; set; }

        [JsonProperty("n")] public char Type { get; set; }

        [JsonProperty("v")] public AnimatedValue Length { get; set; }
    }

    enum LineCap
    {
        Butt = 1,
        Round = 2,
        Square = 3,
    }

    class TransformShape : Shape
    {
        // Multiple inheritance is a pain here, so lets encapsulate the transform instead.
        // This will be manually deserialized by the converter.
        public Transform Transform { get; set; }
    }

    class TrimShape : ModifierShape
    {
        [JsonProperty("s")] [JsonRequired] public AnimatedValue Start { get; set; }

        [JsonProperty("e")] [JsonRequired] public AnimatedValue End { get; set; }

        [JsonProperty("o")] [JsonRequired] public AnimatedValue Offset { get; set; }

        [JsonProperty("m")] public TrimMultipleShapes? Multiple { get; set; }
    }

    enum TrimMultipleShapes
    {
        Individually = 1,
        Simultaneously = 2,
    }

    class TwistShape : Shape
    {
        [JsonProperty("a")] public AnimatedValue Angle { get; set; }

        [JsonProperty("c")] public AnimatedVector Size { get; set; }
    }

    class MergeShape : Shape
    {
        [JsonProperty("mm")] public MergeMode? Mode { get; set; }
    }

    enum MergeMode
    {
        Normal = 1,
        Add = 2,
        Subtract = 3,
        Intersect = 4,
        ExcludeIntersections = 5,
    }

    class OffsetPathShape : Shape
    {
        [JsonProperty("a")] public AnimatedValue Amount { get; set; }

        [JsonProperty("lj")] public LineJoin? LineJoin { get; set; }

        [JsonProperty("ml")] public AnimatedValue MiterLimit { get; set; }
    }

    enum LineJoin
    {
        Miter = 1,
        Round = 2,
        Bevel = 3,
    }

    class ZigZagShape : Shape
    {
        [JsonProperty("r")] public AnimatedValue Frequency { get; set; }

        [JsonProperty("s")] public AnimatedValue Amplitude { get; set; }

        [JsonProperty("pt")] public AnimatedValue PointType { get; set; }
    }

    class NoStyleShape : Shape
    {
    }
}