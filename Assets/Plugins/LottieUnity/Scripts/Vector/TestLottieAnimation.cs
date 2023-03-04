using System.Collections.Generic;
using System.IO;
using Unity.VectorGraphics;
using UnityEngine;

namespace Lottie.Vector
{
    public class TestLottieAnimation : MonoBehaviour
    {
        private void Awake()
        {
            // Simple test SVG with overlapping circles
            var svg = @"
                <svg width=""100"" height=""100"" viewBox=""0 0 100 100"" xmlns=""http://www.w3.org/2000/svg"">
                    <g>
                        <circle cx=""50"" cy=""50"" r=""50"" fill=""#000000""/>
                        <circle cx=""50"" cy=""50"" r=""40"" fill=""#ffffff""/>
                        <circle cx=""50"" cy=""50"" r=""30"" fill=""#000000""/>
                        <circle cx=""50"" cy=""50"" r=""20"" fill=""#ffffff""/>
                        <circle cx=""50"" cy=""50"" r=""10"" fill=""#000000""/>
                        <circle cx=""50"" cy=""50"" r=""5"" fill=""#ffffff""/>
                        <circle cx=""50"" cy=""50"" r=""2"" fill=""#000000""/>
                    </g>
                </svg>
            ";
                
            var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));

            var animators = new List<AnimatedNode>();

            // From 1-6, rotate the circles by 1 degree
            for (var i = 1; i <= 6; i++)
            {
                var animatedNode = new AnimatedNode(sceneInfo.Scene.Root.Children[0].Children[i]);
                var rotOffset = (i / 6f) * 2f * Mathf.PI;
                animatedNode.Register((node, time) =>
                {
                    node.Transform = node.Transform.RotationWithAnchor(new Vector2(50, 52f), (float)(time * 2f * Mathf.PI + rotOffset));
                    return true;
                });
                animators.Add(animatedNode);
            }

            var lottieAnim = new LottieVectorAnimation(sceneInfo.Scene, animators, 0, 60, 30);
            
            // Create LottieVectorController
            var controller = gameObject.AddComponent<LottieVectorController>();
            controller.SetAnimation(lottieAnim);
        }
    }
}