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
using Structure.Sketching.Filters.Interfaces;
using Structure.Sketching.Numerics;
using System;
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Smoothing;

/// <summary>
/// SNN Blur on an image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class SnnBlur : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SnnBlur"/> class.
    /// </summary>
    /// <param name="apetureRadius">The apeture radius.</param>
    public SnnBlur(int apetureRadius)
    {
        ApetureRadius = apetureRadius;
    }

    /// <summary>
    /// Gets or sets the apeture radius.
    /// </summary>
    /// <value>The apeture radius.</value>
    public int ApetureRadius { get; set; }

    /// <summary>
    /// Applies the filter to the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The image</returns>
    public unsafe Image Apply(Image image, Rectangle targetLocation = default)
    {
        targetLocation = targetLocation == default ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
        var tempValues = new Color[image.Pixels.Length];
        Array.Copy(image.Pixels, tempValues, tempValues.Length);
        var apetureMin = -ApetureRadius;
        var apetureMax = ApetureRadius;
        Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
        {
            fixed (Color* targetPointer = &tempValues[y * image.Width + targetLocation.Left])
            {
                var targetPointer2 = targetPointer;
                for (var x = targetLocation.Left; x < targetLocation.Right; ++x)
                {
                    uint rValue = 0;
                    uint gValue = 0;
                    uint bValue = 0;
                    var numPixels = 0;
                    for (var x2 = apetureMin; x2 < apetureMax; ++x2)
                    {
                        var tempX1 = x + x2;
                        var tempX2 = x - x2;
                        if (tempX1 >= targetLocation.Left && tempX1 < targetLocation.Right && tempX2 >= targetLocation.Left && tempX2 < targetLocation.Right)
                        {
                            for (var y2 = apetureMin; y2 < apetureMax; ++y2)
                            {
                                var tempY1 = y + y2;
                                var tempY2 = y - y2;
                                if (tempY1 >= targetLocation.Bottom && tempY1 < targetLocation.Top && tempY2 >= targetLocation.Bottom && tempY2 < targetLocation.Top)
                                {
                                    var tempValue1 = image.Pixels[y * image.Width + x];
                                    var tempValue2 = image.Pixels[tempY1 * image.Width + tempX1];
                                    var tempValue3 = image.Pixels[tempY2 * image.Width + tempX2];
                                    if (Distance.Euclidean(tempValue1, tempValue2) < Distance.Euclidean(tempValue1, tempValue3))
                                    {
                                        rValue += tempValue2.Red;
                                        gValue += tempValue2.Green;
                                        bValue += tempValue2.Blue;
                                    }
                                    else
                                    {
                                        rValue += tempValue3.Red;
                                        gValue += tempValue3.Green;
                                        bValue += tempValue3.Blue;
                                    }
                                    ++numPixels;
                                }
                            }
                        }
                    }
                    tempValues[y * image.Width + x].Red = (byte)(rValue / numPixels);
                    tempValues[y * image.Width + x].Green = (byte)(gValue / numPixels);
                    tempValues[y * image.Width + x].Blue = (byte)(bValue / numPixels);
                    tempValues[y * image.Width + x].Alpha = image.Pixels[y * image.Width + x].Alpha;
                }
            }
        });
        return image.ReCreate(image.Width, image.Height, tempValues);
    }
}