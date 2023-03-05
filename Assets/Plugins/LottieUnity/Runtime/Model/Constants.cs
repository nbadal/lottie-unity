namespace Lottie.Model
{
    public enum MatteMode
    {
        Normal = 0,
        Alpha = 1,
        InvertedAlpha = 2,
        Luma = 3,
        InvertedLuma = 4,
    }

    public enum LayerType
    {
        PreComp = 0,
        Solid = 1,
        Image = 2,
        Null = 3,
        Shape = 4,
        Text = 5,
        Audio = 6,
        VideoPlaceholder = 7,
        ImageSequence = 8,
        Video = 9,
        ImagePlaceholder = 10,
        Guide = 11,
        Adjustment = 12,
        Camera = 13,
        Light = 14,
        Data = 15,
    }

    public enum BlendMode
    {
        Normal = 0,
        Multiply = 1,
        Screen = 2,
        Overlay = 3,
        Darken = 4,
        Lighten = 5,
        ColorDodge = 6,
        ColorBurn = 7,
        HardLight = 8,
        SoftLight = 9,
        Difference = 10,
        Exclusion = 11,
        Hue = 12,
        Saturation = 13,
        Color = 14,
        Luminosity = 15,
        Add = 16,
        HardMix = 17,
    }
}