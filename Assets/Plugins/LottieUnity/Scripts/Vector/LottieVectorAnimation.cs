using System.Collections.Generic;
using Unity.VectorGraphics;

namespace Lottie.Vector
{
    public class LottieVectorAnimation
    {
        public Scene Scene { get; }
        public List<AnimatedNode> Animators { get; }
        public double InPoint { get; }
        public double OutPoint { get; }
        public double FrameRate { get; }

        public LottieVectorAnimation(Scene scene, List<AnimatedNode> animators, double inPoint, double outPoint,
            double frameRate)
        {
            Scene = scene;
            Animators = animators;
            InPoint = inPoint;
            OutPoint = outPoint;
            FrameRate = frameRate;
        }
    }
}