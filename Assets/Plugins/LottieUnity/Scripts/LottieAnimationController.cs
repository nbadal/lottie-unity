using System;
using Lottie.Model;
using UnityEngine;

namespace Lottie
{
    public class LottieAnimationController : MonoBehaviour
    {
        [SerializeField]
        private TextAsset animationAsset;
        
        private bool _needsRebuild;

        private void Start()
        {
            _needsRebuild = true;
            BuildLottie();
        }

        private void Update()
        {
            BuildLottie();
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            _needsRebuild = true;
        }
        #endif

        private void BuildLottie()
        {
            if (!_needsRebuild) return;
            
            // Clear all children
            for (var i = 0; i < transform.childCount; i++) DestroyImmediate(transform.GetChild(i).gameObject);
            
            if (!enabled) return;
            
            if (animationAsset == null) return;
            
            try
            {
                // Load the animation
                var lottie = LottieAnimation.FromJson(animationAsset.text);

                // Build the animation objects
                Builder.Build(this, lottie);
            } catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }
            
            _needsRebuild = false;
        }

    }
}