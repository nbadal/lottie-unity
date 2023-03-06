using System.IO;
using System.Linq;
using Lottie.Vector;
using NUnit.Framework;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.TestTools.Graphics;

public class ImageComparisonTest
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Wipe "ActualImages" folder
        var imagesPath = Application.dataPath + "/ActualImages";
        if (Directory.Exists(imagesPath)) Directory.Delete(imagesPath, true);
        var imagesMeta = imagesPath + ".meta";
        if (File.Exists(imagesMeta)) File.Delete(imagesMeta);
    }

    [Test, TestCaseSource(nameof(GetComparisonNames))]
    public void CompareSvg(string name)
    {
        name = name.Replace("--", "/");
        var svg = SVGParser.ImportSVG(new StringReader(Resources.Load<TextAsset>($"compare/{name}.svg").text));
        var lottie = LottieParser.Parse(Resources.Load<TextAsset>($"compare/{name}").text);

        var txtSvg = RenderSceneTexture2D(svg.Scene);
        var txtLottie = RenderSceneTexture2D(lottie.Scene);

        var settings = new ImageComparisonSettings
        {
            AverageCorrectnessThreshold = 0.0005f,
        };
        ImageAssert.AreEqual(txtSvg, txtLottie, settings);
    }

    public static string[] GetComparisonNames()
    {
        var comparisonsPath = Application.dataPath + "/Plugins/LottieUnity/Tests/Resources/compare/";
        return Directory.GetFiles(comparisonsPath, "*.svg.txt", SearchOption.AllDirectories)
            .Select(absPath => absPath.Substring(comparisonsPath.Length, absPath.Length - comparisonsPath.Length))
            .Select(relPath => relPath.Substring(0, relPath.IndexOf('.', relPath.LastIndexOf('/'))))
            .Select(name => name.Replace("/", "--"))
            .ToArray();
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
        return VectorUtils.RenderSpriteToTexture2D(sprite, 512, 512, mat);
    }
}