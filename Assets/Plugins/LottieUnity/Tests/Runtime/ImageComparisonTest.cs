using System;
using System.IO;
using Lottie.Vector;
using NUnit.Framework;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.TestTools.Graphics;

public class ImageComparisonTest
{
    [Test]
    public void CompareSvg()
    {
        var svg = SVGParser.ImportSVG(new StringReader(Resources.Load<TextAsset>("svg/circle.svg").text));
        var lottie = LottieParser.Parse(Resources.Load<TextAsset>("lottie/circle").text);

        var txtSvg = RenderSceneTexture2D(svg.Scene);
        var txtLottie = RenderSceneTexture2D(lottie.Scene);

        ImageAssert.AreEqual(txtSvg, txtLottie);
    }

    private static Texture2D RenderSceneTexture2D(Scene svgScene)
    {
        var tessOpts = new VectorUtils.TessellationOptions
        {
            StepDistance = 1.0f,
            MaxCordDeviation = float.MaxValue,
            MaxTanAngleDeviation = Mathf.PI / 2.0f,
            SamplingStepSize = 0.1f
        };
        var geoms = VectorUtils.TessellateScene(svgScene, tessOpts);
        var sprite = VectorUtils.BuildSprite(geoms, 100.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
        var mat = new Material(Shader.Find("Unlit/Vector"));
        return VectorUtils.RenderSpriteToTexture2D(sprite, 100, 100, mat);
    }
}