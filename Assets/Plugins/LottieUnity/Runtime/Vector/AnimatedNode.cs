using System;
using System.Collections.Generic;
using Unity.VectorGraphics;

namespace Lottie.Vector
{
    /// <summary>
    /// Represents a scene node and any animations that should be applied to it.
    /// </summary>
    public class AnimatedNode
    {
        private readonly SceneNode _node;
        private readonly List<Func<SceneNode, double, bool>> _animators = new List<Func<SceneNode, double, bool>>();

        public AnimatedNode(SceneNode node)
        {
            _node = node;
        }

        public void Register(Func<SceneNode, double, bool> animator)
        {
            _animators.Add(animator);
        }

        public bool Update(double time)
        {
            var needsRetessellation = false;
            foreach (var animation in _animators)
            {
                needsRetessellation |= animation.Invoke(_node, time);
            }

            return needsRetessellation;
        }
    }
}