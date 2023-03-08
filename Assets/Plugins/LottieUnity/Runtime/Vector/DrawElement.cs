using System.Collections.Generic;
using Unity.VectorGraphics;

namespace Lottie.Vector
{
    public abstract class DrawElement
    {
        public readonly List<BezierContour> Contours = new List<BezierContour>();
        public bool IsConvex = true;
        
        protected internal abstract void ApplyToShape(ref Shape shape);

        public void AddContour(Shape shape)
        {
            Contours.AddRange(shape.Contours);
            IsConvex &= shape.IsConvex;
        }
        
        public virtual void AddElement(DrawElement element)
        {
            Contours.AddRange(element.Contours);
            IsConvex &= element.IsConvex;
        }
    }
    
    public class GroupElement : DrawElement
    {
        public readonly List<DrawElement> Elements = new List<DrawElement>();
        public Matrix2D Transform { get; private set;  }

        public void AddGroup(GroupElement group)
        {
            Elements.ForEach(e => e.Contours.AddRange(group.Contours));
            Elements.Add(group);
            Contours.AddRange(group.Contours);
        }

        public void AddShape(Shape shape)
        {
            Elements.ForEach(e => e.AddContour(shape));
            Contours.AddRange(shape.Contours);
        }

        public override void AddElement(DrawElement element)
        {
            base.AddElement(element);
            Elements.Add(element);
        }

        protected internal override void ApplyToShape(ref Shape shape)
        {
            // Nothing to do
        }

        public void SetTransform(Matrix2D transform)
        {
            Transform = transform;
        }
    }
    
    public class FillElement : DrawElement
    {
        private readonly IFill _fill;

        public FillElement(IFill fill)
        {
            _fill = fill;
        }

        protected internal override void ApplyToShape(ref Shape shape)
        {
            shape.Fill = _fill;
        }
    }

    public class PathElement : DrawElement
    {
        private readonly PathProperties _pathProps;
        
        public PathElement(PathProperties pathProps)
        {
            _pathProps = pathProps;
        }

        protected internal override void ApplyToShape(ref Shape shape)
        {
            shape.PathProps = _pathProps;
        }
    }
}
