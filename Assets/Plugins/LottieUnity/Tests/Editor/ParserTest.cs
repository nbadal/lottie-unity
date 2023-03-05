using System;
using Lottie.Vector;
using NUnit.Framework;
using UnityEngine;

public class ParserTest
{
    [Test]
    public void SimpleJsonCanParse()
    {
        var lottie = LottieParser.Parse(Resources.Load<TextAsset>("lottie/circle").text);
        Assert.IsNotNull(lottie);
    }

    [Test]
    public void InvalidJsonFails()
    {
        try
        {
            LottieParser.Parse("this isn't json");
            Assert.Fail("Expected exception");
        }
        catch (Exception)
        {
            // success
        }
    }
}