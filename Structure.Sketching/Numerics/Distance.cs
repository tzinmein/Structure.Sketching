using Structure.Sketching.Colors;
using System;

namespace Structure.Sketching.Numerics;

/// <summary>
/// Contains distance related utilities
/// </summary>
public static class Distance
{
    /// <summary>
    /// Gets the Euclidean distance between two colors
    /// </summary>
    /// <param name="color1">The color1.</param>
    /// <param name="color2">The color2.</param>
    /// <returns>The distance between the colors</returns>
    public static double Euclidean(Color color1, Color color2)
    {
        int red = color1.Red - color2.Red;
        int green = color1.Green - color2.Green;
        int blue = color1.Blue - color2.Blue;
        int alpha = color1.Alpha - color2.Alpha;
        red *= red;
        green *= green;
        blue *= blue;
        alpha *= alpha;

        return Math.Sqrt(red + green + blue + alpha);
    }
}