using System;
using System.Collections.Generic;
using Lottie.Model;
using Unity.VectorGraphics;
using UnityEngine;

namespace Lottie.Vector
{
    public static class TransformUtils
    {
        public static Matrix2D ToMatrix2D(this Model.Transform t, in List<AnimatedNode> animators)
        {
            // TODO: Add animation support
            var transform = Matrix2D.identity;
            if (t.IsAnimated())
            {
                throw new NotImplementedException();
            }

            // Anchor point offset
            if (t.AnchorPoint != null)
            {
                var anchor = t.AnchorPoint.Value;
                transform = Matrix2D.Translate(new Vector2((float)-anchor[0], (float)-anchor[1])) * transform;
            }

            // Scale
            if (t.Scale != null)
            {
                var scale = new Vector2((float)t.Scale.Value[0], (float)t.Scale.Value[1])/100f;
                transform = Matrix2D.Scale(scale) * transform;
            }

            // Rotation
            switch (t)
            {
                case AngleRotationTransform art:
                    if (art.Rotation == null) break;
                    var rotation = (float)(art.Rotation.Value ?? 0);
                    var rotationDegrees = rotation * Mathf.Deg2Rad;
                    transform = Matrix2D.RotateRH(rotationDegrees) * transform;
                    break;
                case SplitRotationTransform _:
                    // TODO: calculate angular rotation
                    throw new NotImplementedException();
            }

            // Translation
            if (t.Position != null)
            {
                switch (t.Position)
                {
                    case AnimatedPosition ap:
                        var position = ap.Value;
                        transform = Matrix2D.Translate(new Vector2((float)position[0], (float)position[1])) *
                                    transform;
                        break;
                    case AnimatedSplitVector asv:
                        var x = (float)(asv.X.Value ?? 0);
                        var y = (float)(asv.Y.Value ?? 0);
                        // TODO: What do we do with the z value?
                        transform = Matrix2D.Translate(new Vector2(x, y)) * transform;
                        break;
                }
            }

            // TODO: Skew. Where in the order of operations does this go?
            return transform;
        }
    }
}