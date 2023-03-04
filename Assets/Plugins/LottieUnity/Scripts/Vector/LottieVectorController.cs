using System;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;

namespace Lottie.Vector
{
    public enum LoopMode
    {
        Off,
        Loop,
        Mirror
    }

    public class LottieVectorController : MonoBehaviour
    {
        public LoopMode loop = LoopMode.Loop;

        private LottieVectorAnimation _lottieAnimation;

        public bool Playing
        {
            get => _startTime > _pauseTime;
            set
            {
                if (value)
                {
                    _startTime = Time.time;
                }
                else
                {
                    _pauseTime = Time.time;
                }
            }
        }

        private bool _firstRender = true;
        private double _lastAnimationTime = -1;
        private double _startTime;
        private double _pauseTime;

        private readonly VectorUtils.TessellationOptions _tessellationOptions = new VectorUtils.TessellationOptions()
        {
            StepDistance = 1.0f,
            MaxCordDeviation = float.MaxValue,
            MaxTanAngleDeviation = Mathf.PI / 2.0f,
            SamplingStepSize = 0.1f
        };

        public void SetAnimation(LottieVectorAnimation lottie)
        {
            _lottieAnimation = lottie;
            _startTime = Time.time;
            _pauseTime = 0;
        }

        private void Update()
        {
            if (_lottieAnimation == null) return;

            // Skip rendering if we've already shown the calculated frame
            if (Math.Abs(_lastAnimationTime - AnimationTime) < 0.01) return;
            _lastAnimationTime = AnimationTime;

            if (!Animate() && !_firstRender) return;
            _firstRender = false;

            var geoms = VectorUtils.TessellateScene(_lottieAnimation.Scene, _tessellationOptions);
            var sprite = VectorUtils.BuildSprite(geoms, 10.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
            GetComponent<SpriteRenderer>().sprite = sprite;
        }

        /// <summary>
        /// Update the animations within the scene.
        /// </summary>
        /// <returns>True if the scene needs to be retessellated as a result.</returns>
        private bool Animate()
        {
            // Update the animations, return true if any of them need to retessellate
            return _lottieAnimation.Animators.Aggregate(false, (needsUpdate, animator) =>
                needsUpdate | animator.Update(AnimationTime / AnimationDuration)
            );
        }

        private double AnimationFrame => AnimationTime * _lottieAnimation.FrameRate;

        private double AnimationTime
        {
            get
            {
                double animationTime;
                if (_pauseTime > _startTime)
                {
                    animationTime = _pauseTime - _startTime;
                }
                else
                {
                    animationTime = Time.time - _startTime;
                }

                if (animationTime < AnimationDuration) return animationTime;
                switch (loop)
                {
                    default:
                    case LoopMode.Off:
                        // Render last frame
                        animationTime = AnimationDuration;
                        break;
                    case LoopMode.Loop:
                        // Loop the animation
                        animationTime %= AnimationDuration;
                        break;
                    case LoopMode.Mirror:
                        // Loop the animation, but reverse every other loop
                        var loopCount = (int)(animationTime / AnimationDuration);
                        animationTime %= AnimationDuration;
                        if (loopCount % 2 == 1)
                        {
                            animationTime = AnimationDuration - animationTime;
                        }

                        break;
                }

                return animationTime;
            }
        }

        private double AnimationDuration
        {
            get
            {
                var animationDuration =
                    (_lottieAnimation.OutPoint - _lottieAnimation.InPoint) / _lottieAnimation.FrameRate;
                return animationDuration;
            }
        }
    }
}