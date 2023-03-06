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
        [JsonProperty("nm")] public string Name;

        [JsonProperty("mn")] public string MatchName;

        [JsonProperty("hd")] public bool? Hidden;

        [JsonProperty("ty")] [JsonRequired] public string Type;

        [JsonProperty("bm")] public BlendMode? BlendMode;

        [JsonProperty("ix")] public int? PropertyIndex;

        [JsonProperty("cl")] public string CssClass;

        [JsonProperty("ln")] public string LayerXmlId;
    }

    public class ShapeWithDirection : Shape
    {
        [JsonProperty("d")] public ShapeDirection Direction;
    }

    class EllipseShape : ShapeWithDirection
    {
        [JsonProperty("p")] [JsonRequired] public AnimatedPosition Position;

        [JsonProperty("s")] [JsonRequired] public AnimatedVector Size;
    }

    class FillShape : Shape
    {
        [JsonProperty("o")] [JsonRequired] public AnimatedValue Opacity;

        [JsonProperty("c")] [JsonRequired] public AnimatedColor Color;

        [JsonProperty("r")] public FillRule? FillRule;
    }

    class GradientShape : Shape
    {
        [JsonProperty("s")] [JsonRequired] public AnimatedVector StartPoint;

        [JsonProperty("e")] [JsonRequired] public AnimatedVector EndPoint;

        [JsonProperty("g")] [JsonRequired] public AnimatedGradient Colors;

        [JsonProperty("t")] public GradientType? GradientType;

        // Radial:

        [JsonProperty("h")] public AnimatedValue HighlightLength;

        [JsonProperty("a")] public AnimatedValue HighlightAngle;
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
        [JsonProperty("o")] [JsonRequired] public AnimatedValue Opacity;

        [JsonProperty("r")] public FillRule? FillRule;
    }

    class GradientStrokeShape : GradientShape
    {
        [JsonProperty("lc")] public LineCap? LineCap;

        [JsonProperty("lj")] public LineJoin? LineJoin;

        [JsonProperty("ml")] public double? MiterLimit;

        [JsonProperty("ml2")] public AnimatedValue AnimatedMiterLimit;

        [JsonProperty("o")] [JsonRequired] public AnimatedValue Opacity;

        [JsonProperty("w")] [JsonRequired] public AnimatedValue StrokeWidth;

        [JsonProperty("d")] public List<StrokeDash> Dashes;
    }

    class GroupShape : Shape
    {
        [JsonProperty("np")] public double? NumProperties;

        [JsonProperty("it")] public List<Shape> Shapes;

        [JsonProperty("cix")] public int? PropertyCIndex;
    }

    class PathShape : ShapeWithDirection
    {
        [JsonProperty("ks")] [JsonRequired] public AnimatedBezier Shape;
    }

    class PolystarShape : ShapeWithDirection
    {
        [JsonProperty("p")] [JsonRequired] public AnimatedPosition Position;

        [JsonProperty("or")] [JsonRequired] public AnimatedValue OuterRadius;

        [JsonProperty("os")] [JsonRequired] public AnimatedValue OuterRoundness;

        [JsonProperty("r")] [JsonRequired] public AnimatedValue Rotation;

        [JsonProperty("pt")] [JsonRequired] public AnimatedValue Points;

        [JsonProperty("sy")] public StarType? StarType;

        // TODO: required if StarType is Star:

        [JsonProperty("ir")] public AnimatedValue InnerRadius;

        [JsonProperty("is")] public AnimatedValue InnerRoundness;
    }

    enum StarType
    {
        Star = 1,
        Polygon = 2,
    }

    class PuckerBloatShape : Shape
    {
        [JsonProperty("a")] public AnimatedValue Amount;
    }

    class RectangleShape : ShapeWithDirection
    {
        [JsonProperty("p")] [JsonRequired] public AnimatedPosition Position;

        [JsonProperty("s")] [JsonRequired] public AnimatedVector Size;

        [JsonProperty("r")] [JsonRequired] public AnimatedValue Radius;
    }

    class ModifierShape : Shape
    {
    }

    class RepeaterShape : ModifierShape
    {
        [JsonProperty("c")] [JsonRequired] public AnimatedValue Copies;

        [JsonProperty("o")] public AnimatedValue Offset;

        [JsonProperty("m")] public Composite? Composite;

        [JsonProperty("tr")] [JsonRequired] public List<RepeaterTransform> Transform;
    }

    class RepeaterTransform
    {
        // TODO: Transform properties

        [JsonProperty("so")] public AnimatedValue StartOpacity;

        [JsonProperty("eo")] public AnimatedValue EndOpacity;
    }

    enum Composite
    {
        Above = 1,
        Below = 2,
    }

    class RoundedCornersShape : ModifierShape
    {
        [JsonProperty("r")] [JsonRequired] public AnimatedValue Radius;
    }

    class StrokeShape : Shape
    {
        [JsonProperty("lc")] public LineCap? LineCap;

        [JsonProperty("lj")] public LineJoin? LineJoin;

        [JsonProperty("ml")] public double? MiterLimit;

        [JsonProperty("ml2")] public AnimatedValue AnimatedMiterLimit;

        [JsonProperty("o")] [JsonRequired] public AnimatedValue Opacity;

        [JsonProperty("w")] [JsonRequired] public AnimatedValue StrokeWidth;

        [JsonProperty("d")] public List<StrokeDash> Dashes;

        [JsonProperty("c")] [JsonRequired] public AnimatedColor Color;
    }

    class StrokeDash
    {
        [JsonProperty("nm")] public string Name;

        [JsonProperty("mn")] public string MatchName;

        [JsonProperty("n")] public char Type;

        [JsonProperty("v")] public AnimatedValue Length;
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
        public Transform Transform;
    }

    class TrimShape : ModifierShape
    {
        [JsonProperty("s")] [JsonRequired] public AnimatedValue Start;

        [JsonProperty("e")] [JsonRequired] public AnimatedValue End;

        [JsonProperty("o")] [JsonRequired] public AnimatedValue Offset;

        [JsonProperty("m")] public TrimMultipleShapes? Multiple;
    }

    enum TrimMultipleShapes
    {
        Individually = 1,
        Simultaneously = 2,
    }

    class TwistShape : Shape
    {
        [JsonProperty("a")] public AnimatedValue Angle;

        [JsonProperty("c")] public AnimatedVector Size;
    }

    class MergeShape : Shape
    {
        [JsonProperty("mm")] public MergeMode? Mode;
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
        [JsonProperty("a")] public AnimatedValue Amount;

        [JsonProperty("lj")] public LineJoin? LineJoin;

        [JsonProperty("ml")] public AnimatedValue MiterLimit;
    }

    enum LineJoin
    {
        Miter = 1,
        Round = 2,
        Bevel = 3,
    }

    class ZigZagShape : Shape
    {
        [JsonProperty("r")] public AnimatedValue Frequency;

        [JsonProperty("s")] public AnimatedValue Amplitude;

        [JsonProperty("pt")] public AnimatedValue PointType;
    }

    class NoStyleShape : Shape
    {
    }
}