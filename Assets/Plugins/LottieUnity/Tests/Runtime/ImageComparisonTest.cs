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
        var actualImagesPath = Application.dataPath + "/ActualImages";
        if (Directory.Exists(actualImagesPath)) Directory.Delete(actualImagesPath, true);
    }

    [Test, TestCaseSource(nameof(GetComparisonNames))]
    public void CompareSvg(string name)
    {
        var svg = SVGParser.ImportSVG(new StringReader(Resources.Load<TextAsset>($"svg/{name}.svg").text));
        var lottie = LottieParser.Parse(Resources.Load<TextAsset>($"lottie/{name}").text);

        var txtSvg = RenderSceneTexture2D(svg.Scene);
        var txtLottie = RenderSceneTexture2D(lottie.Scene);

        var settings = new ImageComparisonSettings
        {
            AverageCorrectnessThreshold = 0.0005f,
        };
        ImageAssert.AreEqual(txtSvg, txtLottie, settings);
    }

    public static string[] GetComparisonNames() =>
        Directory.GetFiles(Application.dataPath + "/Resources/svg", "*.svg.txt", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Select(filename => filename.Substring(0, filename.IndexOf('.')))
            .ToArray();

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