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
using Structure.Sketching.Filters.Interfaces;
using Structure.Sketching.Numerics;
using System;
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Morphology;

/// <summary>
/// Dilates an image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class Dilate : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Dilate"/> class.
    /// </summary>
    /// <param name="apertureRadius">The aperture radius.</param>
    public Dilate(int apertureRadius)
    {
        ApertureRadius = apertureRadius;
    }

    /// <summary>
    /// Gets or sets the aperture radius.
    /// </summary>
    /// <value>The aperture radius.</value>
    public int ApertureRadius { get; set; }

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
        var apertureMin = -ApertureRadius;
        var apertureMax = ApertureRadius;
        Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
        {
            fixed (Color* targetPointer = &image.Pixels[y * image.Width + targetLocation.Left])
            {
                var targetPointer2 = targetPointer;
                for (var x = targetLocation.Left; x < targetLocation.Right; ++x)
                {
                    byte rValue = 0;
                    byte gValue = 0;
                    byte bValue = 0;
                    for (var y2 = apertureMin; y2 < apertureMax; ++y2)
                    {
                        var tempY = y + y2;
                        var tempX = x + apertureMin;
                        if (tempY < 0 || tempY >= image.Height) continue;
                        var length = ApertureRadius * 2;
                        if (tempX < 0)
                        {
                            length += tempX;
                            tempX = 0;
                        }
                        var start = tempY * image.Width + tempX;
                        fixed (Color* imagePointer = &tempValues[start])
                        {
                            var imagePointer2 = imagePointer;
                            for (var x2 = 0; x2 < length; ++x2)
                            {
                                if (tempX >= image.Width)
                                    break;
                                var tempR = (*imagePointer2).Red;
                                var tempG = (*imagePointer2).Green;
                                var tempB = (*imagePointer2).Blue;
                                ++imagePointer2;
                                rValue = rValue > tempR ? rValue : tempR;
                                gValue = gValue > tempG ? gValue : tempG;
                                bValue = bValue > tempB ? bValue : tempB;
                                ++tempX;
                            }
                        }
                    }
                    (*targetPointer2).Red = rValue;
                    (*targetPointer2).Green = gValue;
                    (*targetPointer2).Blue = bValue;
                    ++targetPointer2;
                }
            }
        });
        return image;
    }
}