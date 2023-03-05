using Unity.VectorGraphics;
using UnityEngine;

namespace Lottie
{
    public static class Matrix2DUtils
    {
        public static Matrix2D RotationWithAnchor(this Matrix2D m, Vector2 anchor, float angleRadians)
        {
            return Matrix2D.Translate(anchor)
                   * Matrix2D.RotateLH(angleRadians)
                   * Matrix2D.Translate(-anchor);
        }
    }
}