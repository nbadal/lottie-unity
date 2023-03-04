using System;
using System.Collections.Generic;
using System.Linq;
using Lottie.Model;
using Unity.VectorGraphics;
using UnityEngine;
using Shape = Lottie.Model.Shape;

namespace Lottie.Vector
{
    public static class ShapeParser
    {
        internal static void ParseShapes(this LottieParser parser, List<Shape> shapes, SceneNode layerNode)
        {
            for (var i = shapes.Count - 1; i >= 0; i--)
            {
                var shape = shapes[i];
                switch (shape)
                {
                    case GroupShape gr:
                        var groupNode = new SceneNode
                        {
                            Children = new List<SceneNode>(),
                        };
                        parser.ParseGroupShapes(gr.Shapes, groupNode);
                        layerNode.Children.Add(groupNode);
                        break;
                    case RectangleShape rc:
                    case EllipseShape el:
                    case PolystarShape sr:
                    case PathShape sh:
                    case FillShape fl:
                    case StrokeShape st:
                    case GradientFillShape gf:
                    case GradientStrokeShape gs:
                    case NoStyleShape no:
                    case TransformShape tr:
                    case RepeaterShape rp:
                    case TrimShape tm:
                    case RoundedCornersShape rd:
                    case PuckerBloatShape pb:
                    case MergeShape mm:
                    case TwistShape tw:
                    case OffsetPathShape op:
                    case ZigZagShape zz:
                        throw new NotImplementedException();
                }
            }
        }

        private static void ParseGroupShapes(this LottieParser parser, List<Shape> grShapes, SceneNode groupNode)
        {
            IFill fill = null;
            PathProperties? pathProps = null;
            Matrix2D? transform = null;

            // Iterate through shapes in reverse order
            for (var i = grShapes.Count - 1; i >= 0; i--)
            {
                var shape = grShapes[i];
                switch (shape)
                {
                    case GroupShape gr:
                        var innerGroupNode = new SceneNode
                        {
                            Children = new List<SceneNode>(),
                        };
                        parser.ParseGroupShapes(gr.Shapes, innerGroupNode);
                        groupNode.Children.Add(innerGroupNode);
                        break;
                    case TransformShape t:
                        // TODO: these animators will need to be proxied back to the resulting shape
                        transform = t.Transform.ToMatrix2D(parser._animators);
                        break;
                    case FillShape fl:
                        fill = new SolidFill
                        {
                            Color = FromLottieColor(fl.Color)
                        };
                        break;
                    case StrokeShape st:
                        if (st.StrokeWidth.IsAnimated == 1)
                        {
                            throw new NotImplementedException();
                        }


                        var patternOffset = 0f;
                        var pattern = st.Dashes == null ? null : MakeDashPattern(st.Dashes, out patternOffset);

                        pathProps = new PathProperties
                        {
                            Stroke = new Stroke
                            {
                                Color = FromLottieColor(st.Color),
                                HalfThickness = ((float?)st.StrokeWidth.Value ?? 0f) / 2f,
                                Pattern = pattern,
                                PatternOffset = patternOffset,
                            },
                            Corners = st.LineJoin switch
                            {
                                LineJoin.Miter => PathCorner.Tipped,
                                LineJoin.Round => PathCorner.Round,
                                LineJoin.Bevel => PathCorner.Beveled,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                            Head = st.LineCap switch
                            {
                                LineCap.Butt => PathEnding.Chop,
                                LineCap.Round => PathEnding.Round,
                                LineCap.Square => PathEnding.Square,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                            Tail = st.LineCap switch
                            {
                                LineCap.Butt => PathEnding.Chop,
                                LineCap.Round => PathEnding.Round,
                                LineCap.Square => PathEnding.Square,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                        };
                        break;
                    case EllipseShape el:
                        if (el.Size.IsAnimated == 1)
                        {
                            throw new NotImplementedException();
                        }

                        var ellipseShape = new Unity.VectorGraphics.Shape();
                        VectorUtils.MakeEllipseShape(ellipseShape, Vector2.zero, (float)el.Size.Value[0] / 2f,
                            (float)el.Size.Value[1] / 2f);

                        if (fill != null) ellipseShape.Fill = fill;
                        if (pathProps != null) ellipseShape.PathProps = pathProps.Value;

                        groupNode.Children.Add(new SceneNode
                        {
                            Shapes = new List<Unity.VectorGraphics.Shape> { ellipseShape },
                        });
                        break;
                    case RectangleShape rc:
                        if (rc.Size.IsAnimated == 1)
                        {
                            throw new NotImplementedException();
                        }

                        var rectShape = new Unity.VectorGraphics.Shape();

                        // For whatever reason, rectangles are centered on their position
                        if (rc.Size == null) break;
                        var size = new Vector2((float)rc.Size.Value[0], (float)rc.Size.Value[1]);
                        var position = new Vector2((float)rc.Position.Value[0], (float)rc.Position.Value[1]);
                        position -= size / 2f;

                        if (rc.Radius != null)
                        {
                            if (rc.Radius.IsAnimated == 1)
                            {
                                throw new NotImplementedException();
                            }

                            var radius = (float?)rc.Radius.Value ?? 0f;
                            var radiusVector = new Vector2(radius, radius);
                            VectorUtils.MakeRectangleShape(rectShape, new Rect(position, size), radiusVector,
                                radiusVector, radiusVector, radiusVector);
                        }
                        else
                        {
                            VectorUtils.MakeRectangleShape(rectShape, new Rect(position, size));
                        }

                        if (fill != null) rectShape.Fill = fill;
                        if (pathProps != null) rectShape.PathProps = pathProps.Value;

                        groupNode.Children.Add(new SceneNode
                        {
                            Shapes = new List<Unity.VectorGraphics.Shape> { rectShape },
                        });
                        break;
                }
            }

            if (transform == null) throw new ArgumentException("Missing transform for group");
            groupNode.Transform = transform.Value;
        }

        private static float[] MakeDashPattern(List<StrokeDash> stDashes, out float patternOffset)
        {
            var offsetLength = 0f;
            if (stDashes.Any(it => it.Type == 'o'))
            {
                var offset = stDashes.First(it => it.Type == 'o');
                if (offset.Length != null)
                {
                    if (offset.Length.IsAnimated == 1)
                    {
                        throw new NotImplementedException();
                    }

                    offsetLength = (float?)offset.Length.Value ?? 0f;
                }
            }

            var gapLength = 0f;
            if (stDashes.Any(it => it.Type == 'g'))
            {
                var gap = stDashes.First(it => it.Type == 'g');
                if (gap.Length != null)
                {
                    if (gap.Length.IsAnimated == 1)
                    {
                        throw new NotImplementedException();
                    }

                    gapLength = (float?)gap.Length.Value ?? 0f;
                }
            }

            var dashLength = 0f;
            if (stDashes.Any(it => it.Type == 'd'))
            {
                var dash = stDashes.First(it => it.Type == 'd');
                if (dash.Length != null)
                {
                    if (dash.Length.IsAnimated == 1)
                    {
                        throw new NotImplementedException();
                    }

                    dashLength = (float?)dash.Length.Value ?? 0f;
                }
            }

            patternOffset = offsetLength;
            return new[] { dashLength, gapLength };
        }

        private static UnityEngine.Color FromLottieColor(AnimatedColor color)
        {
            if (color.IsAnimated == 1)
            {
                // TODO: Hopefully this doesn't require a shader
                throw new NotImplementedException();
            }

            return new UnityEngine.Color
            (
                (float)color.Value.R,
                (float)color.Value.G,
                (float)color.Value.B
            );
        }
    }
}