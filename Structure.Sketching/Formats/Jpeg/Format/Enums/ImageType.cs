﻿namespace Structure.Sketching.Formats.Jpeg.Format.Enums;

/// <summary>
/// Image type enum
/// </summary>
public enum ImageType
{
    /// <summary>
    /// The image is grey scale
    /// </summary>
    GreyScale = 1,

    /// <summary>
    /// The image is RGB or YCbCr
    /// </summary>
    Rgb = 3,

    /// <summary>
    /// The image is CMYK or YCbCrK
    /// </summary>
    Cmyk = 4
}