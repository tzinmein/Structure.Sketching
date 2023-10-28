using Structure.Sketching.Colors;
using Structure.Sketching.ExtensionMethods;

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

using Structure.Sketching.Filters.Interfaces;
using Structure.Sketching.Numerics;
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Normalization;

/// <summary>
/// Stretches the contrast of a specific image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class StretchContrast : IFilter
{
    /// <summary>
    /// Applies the filter to the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The image</returns>
    public unsafe Image Apply(Image image, Rectangle targetLocation = default)
    {
        targetLocation = targetLocation == default ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
        var minValue = new byte[3];
        var maxValue = new byte[3];
        GetMinMaxPixel(minValue, maxValue, image);
        Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
        {
            fixed (Color* targetPointer = &image.Pixels[y * image.Width + targetLocation.Left])
            {
                var targetPointer2 = targetPointer;
                for (var x = targetLocation.Left; x < targetLocation.Right; ++x)
                {
                    image.Pixels[y * image.Width + x].Red = Map(image.Pixels[y * image.Width + x].Red, minValue[0], maxValue[0]);
                    image.Pixels[y * image.Width + x].Green = Map(image.Pixels[y * image.Width + x].Green, minValue[1], maxValue[1]);
                    image.Pixels[y * image.Width + x].Blue = Map(image.Pixels[y * image.Width + x].Blue, minValue[2], maxValue[2]);
                }
            }
        });
        return image;
    }

    private void GetMinMaxPixel(byte[] minValue, byte[] maxValue, Image image)
    {
        minValue[0] = minValue[1] = minValue[2] = 255;
        maxValue[0] = maxValue[1] = maxValue[2] = 0;
        for (var x = 0; x < image.Width; ++x)
        {
            for (var y = 0; y < image.Height; ++y)
            {
                var tempR = image.Pixels[y * image.Width + x].Red;
                var tempG = image.Pixels[y * image.Width + x].Green;
                var tempB = image.Pixels[y * image.Width + x].Blue;
                if (minValue[0] > tempR)
                    minValue[0] = tempR;
                if (maxValue[0] < tempR)
                    maxValue[0] = tempR;

                if (minValue[1] > tempG)
                    minValue[1] = tempG;
                if (maxValue[1] < tempG)
                    maxValue[1] = tempG;

                if (minValue[2] > tempB)
                    minValue[2] = tempB;
                if (maxValue[2] < tempB)
                    maxValue[2] = tempB;
            }
        }
    }

    private byte Map(byte v, byte min, byte max)
    {
        float tempVal = v - min;
        tempVal /= max - min;
        tempVal *= 255;
        return (byte)tempVal.Clamp(0, 255);
    }
}