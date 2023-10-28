/*
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
using Structure.Sketching.ExtensionMethods;
using Structure.Sketching.Filters.Interfaces;
using Structure.Sketching.Numerics;
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Effects;

/// <summary>
/// Pixellates an image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class Pixellate : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Pixellate"/> class.
    /// </summary>
    /// <param name="pixelSize">Size of the pixel.</param>
    public Pixellate(int pixelSize)
    {
        PixelSize = pixelSize;
    }

    /// <summary>
    /// Gets or sets the size of the pixel.
    /// </summary>
    /// <value>The size of the pixel.</value>
    public int PixelSize { get; set; }

    /// <summary>
    /// Applies the filter to the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The image</returns>
    public unsafe Image Apply(Image image, Rectangle targetLocation = default)
    {
        targetLocation = targetLocation == default ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
        for (var y = targetLocation.Bottom; y < targetLocation.Top; y += PixelSize)
        {
            var minY = (y - PixelSize / 2).Clamp(targetLocation.Bottom, targetLocation.Top - 1);
            var maxY = (y + PixelSize / 2).Clamp(targetLocation.Bottom, targetLocation.Top - 1);
            fixed (Color* targetPointer = &image.Pixels[y * image.Width + targetLocation.Left])
            {
                var targetPointer2 = targetPointer;
                for (var x = targetLocation.Left; x < targetLocation.Right; x += PixelSize)
                {
                    uint rValue = 0;
                    uint gValue = 0;
                    uint bValue = 0;
                    var minX = (x - PixelSize / 2).Clamp(targetLocation.Left, targetLocation.Right - 1);
                    var maxX = (x + PixelSize / 2).Clamp(targetLocation.Left, targetLocation.Right - 1);
                    var numberPixels = 0;
                    for (var x2 = minX; x2 < maxX; ++x2)
                    {
                        for (var y2 = minY; y2 < maxY; ++y2)
                        {
                            var tempPixel = image.Pixels[y * image.Width + x];
                            rValue += tempPixel.Red;
                            gValue += tempPixel.Green;
                            bValue += tempPixel.Blue;
                            ++numberPixels;
                        }
                    }
                    rValue /= (uint)numberPixels;
                    gValue /= (uint)numberPixels;
                    bValue /= (uint)numberPixels;
                    Parallel.For(minX, maxX, x2 =>
                    {
                        for (var y2 = minY; y2 < maxY; ++y2)
                        {
                            image.Pixels[y2 * image.Width + x2].Red = (byte)rValue;
                            image.Pixels[y2 * image.Width + x2].Green = (byte)gValue;
                            image.Pixels[y2 * image.Width + x2].Blue = (byte)bValue;
                        }
                    });
                }
            }
        }
        return image;
    }
}