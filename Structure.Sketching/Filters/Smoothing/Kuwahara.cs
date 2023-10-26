/*
Copyright 2016 James Craig

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
/// Does smoothing using Kuwahara filter on an image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class Kuwahara : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Kuwahara"/> class.
    /// </summary>
    /// <param name="apetureRadius">The apeture radius.</param>
    public Kuwahara(int apetureRadius)
    {
        ApetureRadius = apetureRadius;
        _apetureMinX = new[] { -ApetureRadius, 0, -ApetureRadius, 0 };
        _apetureMaxX = new[] { 0, ApetureRadius, 0, ApetureRadius };
        _apetureMinY = new[] { -ApetureRadius, -ApetureRadius, 0, 0 };
        _apetureMaxY = new[] { 0, 0, ApetureRadius, ApetureRadius };
    }

    /// <summary>
    /// Gets or sets the apeture radius.
    /// </summary>
    /// <value>The apeture radius.</value>
    public int ApetureRadius { get; set; }

    private readonly int[] _apetureMaxX;
    private readonly int[] _apetureMaxY;
    private readonly int[] _apetureMinX;
    private readonly int[] _apetureMinY;

    /// <summary>
    /// Applies the filter to the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The image</returns>
    public unsafe Image Apply(Image image, Rectangle targetLocation = default)
    {
        targetLocation = targetLocation == default ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
        var result = new Color[image.Pixels.Length];
        Array.Copy(image.Pixels, result, result.Length);
        Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
        {
            fixed (Color* pointer = &image.Pixels[y * image.Width + targetLocation.Left])
            {
                var sourcePointer = pointer;
                for (var x = targetLocation.Left; x < targetLocation.Right; ++x)
                {
                    uint[] rValues = { 0, 0, 0, 0 };
                    uint[] gValues = { 0, 0, 0, 0 };
                    uint[] bValues = { 0, 0, 0, 0 };
                    uint[] numPixels = { 0, 0, 0, 0 };
                    uint[] maxRValue = { 0, 0, 0, 0 };
                    uint[] maxGValue = { 0, 0, 0, 0 };
                    uint[] maxBValue = { 0, 0, 0, 0 };
                    uint[] minRValue = { 255, 255, 255, 255 };
                    uint[] minGValue = { 255, 255, 255, 255 };
                    uint[] minBValue = { 255, 255, 255, 255 };

                    for (var i = 0; i < 4; ++i)
                    {
                        for (var x2 = _apetureMinX[i]; x2 < _apetureMaxX[i]; ++x2)
                        {
                            var tempX = x + x2;
                            if (tempX >= 0 && tempX < image.Width)
                            {
                                for (var y2 = _apetureMinY[i]; y2 < _apetureMaxY[i]; ++y2)
                                {
                                    var tempY = y + y2;
                                    if (tempY >= 0 && tempY < image.Height)
                                    {
                                        rValues[i] += image.Pixels[tempY * image.Width + tempX].Red;
                                        gValues[i] += image.Pixels[tempY * image.Width + tempX].Green;
                                        bValues[i] += image.Pixels[tempY * image.Width + tempX].Blue;

                                        if (image.Pixels[tempY * image.Width + tempX].Red > maxRValue[i])
                                            maxRValue[i] = image.Pixels[tempY * image.Width + tempX].Red;
                                        else if (image.Pixels[tempY * image.Width + tempX].Red < minRValue[i])
                                            minRValue[i] = image.Pixels[tempY * image.Width + tempX].Red;

                                        if (image.Pixels[tempY * image.Width + tempX].Green > maxGValue[i])
                                            maxGValue[i] = image.Pixels[tempY * image.Width + tempX].Green;
                                        else if (image.Pixels[tempY * image.Width + tempX].Green < minGValue[i])
                                            minGValue[i] = image.Pixels[tempY * image.Width + tempX].Green;

                                        if (image.Pixels[tempY * image.Width + tempX].Blue > maxBValue[i])
                                            maxBValue[i] = image.Pixels[tempY * image.Width + tempX].Blue;
                                        else if (image.Pixels[tempY * image.Width + tempX].Blue < minBValue[i])
                                            minBValue[i] = image.Pixels[tempY * image.Width + tempX].Blue;

                                        ++numPixels[i];
                                    }
                                }
                            }
                        }
                    }

                    var j = 0;
                    var minDifference = uint.MaxValue;
                    for (var i = 0; i < 4; ++i)
                    {
                        var currentDifference = maxRValue[i] - minRValue[i] + (maxGValue[i] - minGValue[i]) + (maxBValue[i] - minBValue[i]);
                        if (currentDifference < minDifference && numPixels[i] > 0)
                        {
                            j = i;
                            minDifference = currentDifference;
                        }
                    }
                    rValues[j] = rValues[j] / numPixels[j];
                    gValues[j] = gValues[j] / numPixels[j];
                    bValues[j] = bValues[j] / numPixels[j];

                    result[y * image.Width + x].Red = (byte)rValues[j];
                    result[y * image.Width + x].Green = (byte)gValues[j];
                    result[y * image.Width + x].Blue = (byte)bValues[j];
                }
            }
        });
        return image.ReCreate(image.Width, image.Height, result);
    }
}