using System.Collections.Generic;
using Unity.VectorGraphics;

namespace Lottie.Vector
{
    public abstract class ShapeStyle
    {
        public List<Unity.VectorGraphics.BezierContour> Contours = new List<BezierContour>();
        public bool IsConvex = true;
        public abstract void ApplyToShape(ref Shape shape);

        public void AddShape(Shape shape)
        {
            Contours.AddRange(shape.Contours);
            IsConvex &= shape.IsConvex;
        }

        public void AddToNode(SceneNode node)
        {
            var shapeCopy = new Unity.VectorGraphics.Shape
            {
                Contours = Contours.ToArray(),
                IsConvex = IsConvex,
            };

            ApplyToShape(ref shapeCopy);

            node.Children.Add(new SceneNode
            {
                Shapes = new List<Unity.VectorGraphics.Shape> { shapeCopy },
            });
        }
    }
    
    public class FillStyle : ShapeStyle
    {
        private readonly IFill _fill;

        public FillStyle(IFill fill)
        {
            _fill = fill;
        }

        public override void ApplyToShape(ref Shape shape)
        {
            shape.Fill = _fill;
        }
    }

    public class PathStyle : ShapeStyle
    {
        private readonly PathProperties _pathProps;
        
        public PathStyle(PathProperties pathProps)
        {
            _pathProps = pathProps;
        }
        
        public override void ApplyToShape(ref Shape shape)
        {
            shape.PathProps = _pathProps;
        }
    }
}