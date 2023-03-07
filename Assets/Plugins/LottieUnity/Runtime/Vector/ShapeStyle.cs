using Unity.VectorGraphics;

namespace Lottie.Vector
{
    public abstract class ShapeStyle
    {
        public abstract void ApplyToShape(ref Shape shape);
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