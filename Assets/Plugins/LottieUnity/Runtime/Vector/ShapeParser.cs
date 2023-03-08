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
            var rootElement = ParseGroupShapes(parser, shapes);
            RenderGroup(rootElement, layerNode);
        }
        
        private static void RenderGroup(GroupElement group, SceneNode node)
        {
            var transform = group.Transform;
            node.Transform = transform;

            foreach (var element in group.Elements)
            {
                switch (element)
                {
                    case GroupElement gr:
                        var childNode = new SceneNode
                        {
                            Children = new List<SceneNode>()
                        };
                        node.Children.Add(childNode);
                        RenderGroup(gr, childNode);
                        break;
                    case { } be:
                        var shapeNode = new SceneNode();
                        var shapeCopy = new Unity.VectorGraphics.Shape
                        {
                            Contours = be.Contours.ToArray(),
                            IsConvex = be.IsConvex,
                        };

                        be.ApplyToShape(ref shapeCopy);

                        node.Children.Add(new SceneNode
                        {
                            Shapes = new List<Unity.VectorGraphics.Shape> { shapeCopy },
                        });
                        node.Children.Add(shapeNode);
                        break;
                }
            }
        }

        private static GroupElement ParseGroupShapes(this LottieParser parser, List<Shape> grShapes)
        {
            Matrix2D transform = Matrix2D.identity;

            var element = new GroupElement();

            // Iterate through shapes in reverse order
            for (var i = grShapes.Count - 1; i >= 0; i--)
            {
                var shape = grShapes[i];
                switch (shape)
                {
                    case GroupShape gr:
                        var innerGroup = parser.ParseGroupShapes(gr.Shapes);
                        element.AddGroup(innerGroup);
                        break;
                    case TransformShape t:
                        // TODO: these animators will need to be proxied back to the resulting shape
                        transform = t.Transform.ToMatrix2D(parser._animators);
                        break;
                    case FillShape fl:
                        if (fl.Opacity.IsAnimated == 1)
                        {
                            throw new NotImplementedException();
                        }

                        var fill = new SolidFill
                        {
                            Color = FromLottieColor(fl.Color),
                            Opacity = ((float?)fl.Opacity.Value ?? 100f) / 100f,
                            Mode = fl.FillRule switch
                            {
                                FillRule.NonZero => FillMode.NonZero,
                                FillRule.EvenOdd => FillMode.OddEven,
                                null => FillMode.NonZero,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                        };
                        element.AddElement(new FillElement(fill));
                        break;
                    case StrokeShape st:
                        if (st.StrokeWidth.IsAnimated == 1)
                        {
                            throw new NotImplementedException();
                        }

                        var patternOffset = 0f;
                        var pattern = st.Dashes == null ? null : MakeDashPattern(st.Dashes, out patternOffset);

                        var pathProps = new PathProperties
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
                                null => PathCorner.Round,
                                LineJoin.Miter => PathCorner.Tipped,
                                LineJoin.Round => PathCorner.Round,
                                LineJoin.Bevel => PathCorner.Beveled,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                            Head = st.LineCap switch
                            {
                                null => PathEnding.Round,
                                LineCap.Butt => PathEnding.Chop,
                                LineCap.Round => PathEnding.Round,
                                LineCap.Square => PathEnding.Square,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                            Tail = st.LineCap switch
                            {
                                null => PathEnding.Round,
                                LineCap.Butt => PathEnding.Chop,
                                LineCap.Round => PathEnding.Round,
                                LineCap.Square => PathEnding.Square,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                        };
                        element.AddElement(new PathElement(pathProps));
                        break;
                    case EllipseShape el:
                        if (el.Size.IsAnimated == 1 || el.Position.IsAnimated())
                        {
                            throw new NotImplementedException();
                        }

                        var ellipseShape = new Unity.VectorGraphics.Shape();
                        var ellipsePos = new Vector2((float)el.Position.Value[0], (float)el.Position.Value[1]);
                        VectorUtils.MakeEllipseShape(ellipseShape, ellipsePos,
                            (float)el.Size.Value[0] / 2f, (float)el.Size.Value[1] / 2f);
                        element.AddShape(ellipseShape);
                        break;
                    case RectangleShape rc:
                        if (rc.Size.IsAnimated == 1)
                        {
                            throw new NotImplementedException();
                        }

                        var rectShape = new Unity.VectorGraphics.Shape();

                        if (CreateRectangleShape(rc, rectShape)) break;
                        element.AddShape(rectShape);
                        break;
                    case PathShape sh:
                        if (sh.Shape.IsAnimated == 1)
                        {
                            throw new NotImplementedException();
                        }

                        var pathShape = new Unity.VectorGraphics.Shape();
                        CreatePathShape(sh, pathShape);
                        element.AddShape(pathShape);
                        break;
                    case PolystarShape sr:
                        if (sr.Position.IsAnimated() || sr.Points.IsAnimated == 1 || sr.Rotation.IsAnimated == 1 ||
                            sr.InnerRadius?.IsAnimated == 1 || sr.OuterRadius?.IsAnimated == 1 ||
                            sr.InnerRoundness?.IsAnimated == 1 || sr.OuterRoundness?.IsAnimated == 1)
                        {
                            throw new NotImplementedException();
                        }

                        var polystarShape = new Unity.VectorGraphics.Shape();
                        CreatePolystarShape(sr, polystarShape);
                        element.AddShape(polystarShape);
                        break;
                    case GradientFillShape gf:
                    case GradientStrokeShape gs:
                    case NoStyleShape no:
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
            element.SetTransform(transform);
            return element;
        }

        private static bool CreateRectangleShape(RectangleShape rc, in Unity.VectorGraphics.Shape rectShape)
        {
            // For whatever reason, rectangles are centered on their position
            if (rc.Size == null) return true;
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

            return false;
        }

        private static void CreatePathShape(PathShape sh, in Unity.VectorGraphics.Shape pathShape)
        {
            var shapeData = sh.Shape.Value;
            var bezierSegments = new BezierSegment[shapeData.SegmentCount];
            for (var segment = 0; segment < shapeData.SegmentCount; segment++)
            {
                var bezierData = shapeData.Segment(segment);
                var bezier = new BezierSegment
                {
                    P0 = new Vector2((float)bezierData.Item1[0], (float)bezierData.Item1[1]),
                    P1 = new Vector2((float)bezierData.Item2[0], (float)bezierData.Item2[1]),
                    P2 = new Vector2((float)bezierData.Item3[0], (float)bezierData.Item3[1]),
                    P3 = new Vector2((float)bezierData.Item4[0], (float)bezierData.Item4[1]),
                };
                bezierSegments[segment] = bezier;
            }

            var path = VectorUtils.BezierSegmentsToPath(bezierSegments);
            if (!(shapeData.Closed ?? false))
            {
                // TODO: This feels hacky - probably a cleaner way to do this in the SVG parser
                // Set all elements of the last segment to p0
                var lastSegment = path[path.Length - 1];
                lastSegment.P1 = lastSegment.P0;
                lastSegment.P2 = lastSegment.P0;
                path[path.Length - 1] = lastSegment;
            }

            pathShape.Contours = new[]
            {
                new BezierContour
                {
                    Segments = path,
                    Closed = shapeData.Closed ?? false
                }
            };
        }

        private static void CreatePolystarShape(PolystarShape sr, in Unity.VectorGraphics.Shape polystarShape)
        {
            var points = (float?)sr.Points.Value ?? 0f;
            if (points <= 1) return;

            var type = sr.StarType ?? StarType.Polygon;
            var position = new Vector2((float)sr.Position.Value[0], (float)sr.Position.Value[1]);
            var rotation = (float?)sr.Rotation.Value ?? 0f;
            rotation *= Mathf.Deg2Rad;
            var innerRadius = (float?)sr.InnerRadius?.Value ?? 0f;
            var outerRadius = (float?)sr.OuterRadius?.Value ?? 0f;
            var innerRoundness = (float?)sr.InnerRoundness?.Value ?? 0f;
            innerRoundness /= 100f;
            var outerRoundness = (float?)sr.OuterRoundness?.Value ?? 0f;
            outerRoundness /= 100f;

            var pointLocations = new List<Vector2>();
            var pointInVector = new List<Vector2>();
            var pointOutVector = new List<Vector2>();

            // Add the outer points
            for (var i = 0; i < points; i++)
            {
                var radius = outerRadius;
                var angle = rotation + i * 2 * Mathf.PI / points - Mathf.PI / 2;
                var point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                pointLocations.Add(point);

                var perimSegment = 2 * Mathf.PI * radius / (points * 4);
                var oVec = new Vector2(point.y, -point.x) / Mathf.Sqrt(point.x * point.x + point.y * point.y);
                var tangent = oVec * perimSegment * outerRoundness;
                pointInVector.Add(point + tangent);
                pointOutVector.Add(point - tangent);
            }

            if (type == StarType.Star)
            {
                // Add the inner points
                for (var i = 0; i < points; i++)
                {
                    var radius = innerRadius;
                    var angle = rotation + i * 2 * Mathf.PI / points + Mathf.PI / points - Mathf.PI / 2;
                    var point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                    pointLocations.Insert(i * 2 + 1, point);

                    var perimSegment = 2 * Mathf.PI * radius / (points * 4);
                    var oVec = new Vector2(point.y, -point.x) / Mathf.Sqrt(point.x * point.x + point.y * point.y);
                    var tangent = oVec * perimSegment * innerRoundness;
                    pointInVector.Insert(i * 2 + 1, point + tangent);
                    pointOutVector.Insert(i * 2 + 1, point - tangent);
                }
            }

            var lines = new List<BezierSegment>();
            for (var i = 0; i < pointLocations.Count; i++)
            {
                var nextIdx = (i + 1) % pointLocations.Count;
                var pA = pointLocations[i];
                var pB = pointLocations[nextIdx];

                var pBIn = pointInVector[nextIdx];
                var pAOut = pointOutVector[i];

                var line = new BezierSegment
                {
                    P0 = pA + position,
                    P1 = pAOut + position,
                    P2 = pBIn + position,
                    P3 = pB + position
                };

                lines.Add(line);
            }

            var path = VectorUtils.BezierSegmentsToPath(lines.ToArray());

            polystarShape.Contours = new[]
            {
                new BezierContour
                {
                    Segments = path,
                    Closed = true
                }
            };
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
                (float)color.Value.B,
                (float?)color.Value.A ?? 1f
            );
        }
    }
}
