﻿/*
Copyright 2016 James Craig
Copyright 2023 Ho Tzin Mein

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Structure.Sketching.Colors;
using Structure.Sketching.Formats.Jpeg.Format.Enums;
using Structure.Sketching.Formats.Jpeg.Format.Segments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Structure.Sketching.Formats.Jpeg.Format.Segments.BaseClasses;

namespace Structure.Sketching.Formats.Jpeg.Format.HelperClasses;

/// <summary>
/// Grey image holder
/// </summary>
public class GreyImage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreyImage"/> class.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public GreyImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new byte[width * height];
        Stride = width;
        Offset = 0;
    }

    private GreyImage()
    {
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>The height.</value>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public int Offset { get; set; }

    /// <summary>
    /// Gets or sets the pixels.
    /// </summary>
    /// <value>The pixels.</value>
    public byte[] Pixels { get; set; }

    /// <summary>
    /// Gets or sets the stride.
    /// </summary>
    /// <value>The stride.</value>
    public int Stride { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the x.
    /// </summary>
    /// <value>The x.</value>
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the y.
    /// </summary>
    /// <value>The y.</value>
    public int Y { get; set; }

    /// <summary>
    /// Converts this to an image.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="image">The image.</param>
    /// <param name="segments">The segments.</param>
    /// <returns>Converts to an image.</returns>
    public Image Convert(int width, int height, Image image, IEnumerable<SegmentBase> segments)
    {
        var startFrameSegment = segments.OfType<StartOfFrame>().FirstOrDefault();
        if (startFrameSegment.TypeOfImage == ImageType.GreyScale)
        {
            var pixelWidth = startFrameSegment.Width;
            var pixelHeight = startFrameSegment.Height;

            var pixels = new Color[pixelWidth * pixelHeight];

            Parallel.For(
                0,
                pixelHeight,
                y =>
                {
                    var yoff = GetRowOffset(y);
                    for (var x = 0; x < pixelWidth; x++)
                    {
                        var offset = y * pixelWidth + x;

                        pixels[offset].Red = Pixels[yoff + x];
                        pixels[offset].Green = Pixels[yoff + x];
                        pixels[offset].Blue = Pixels[yoff + x];
                        pixels[offset].Alpha = 255;
                    }
                });

            image.ReCreate(pixelWidth, pixelHeight, pixels);
        }
        return image;
    }

    /// <summary>
    /// Gets the row offset.
    /// </summary>
    /// <param name="y">The y.</param>
    /// <returns>The row offset</returns>
    public int GetRowOffset(int y)
    {
        return Offset + y * Stride;
    }

    /// <summary>
    /// Gets a sub image from the image.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>The resulting sub image</returns>
    public GreyImage SubImage(int x, int y, int width, int height)
    {
        var returnValue = new GreyImage
        {
            Width = width,
            Height = height,
            Pixels = Pixels,
            Stride = Stride,
            Offset = y * Stride + x
        };
        return returnValue;
    }
}