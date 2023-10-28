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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Smoothing;

/// <summary>
/// Medians an image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class Median : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Median"/> class.
    /// </summary>
    /// <param name="apetureRadius">The apeture radius.</param>
    public Median(int apetureRadius)
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
                    var rValues = new List<byte>();
                    var gValues = new List<byte>();
                    var bValues = new List<byte>();
                    for (var x2 = apetureMin; x2 < apetureMax; ++x2)
                    {
                        var tempX = x + x2;
                        if (tempX >= targetLocation.Left && tempX < targetLocation.Right)
                        {
                            for (var y2 = apetureMin; y2 < apetureMax; ++y2)
                            {
                                var tempY = y + y2;
                                if (tempY >= targetLocation.Bottom && tempY < targetLocation.Top)
                                {
                                    rValues.Add(image.Pixels[tempY * image.Width + tempX].Red);
                                    gValues.Add(image.Pixels[tempY * image.Width + tempX].Green);
                                    bValues.Add(image.Pixels[tempY * image.Width + tempX].Blue);
                                }
                            }
                        }
                    }
                    tempValues[y * image.Width + x].Red = rValues.OrderBy(_ => _).ElementAt(rValues.Count / 2);
                    tempValues[y * image.Width + x].Green = gValues.OrderBy(_ => _).ElementAt(rValues.Count / 2);
                    tempValues[y * image.Width + x].Blue = bValues.OrderBy(_ => _).ElementAt(rValues.Count / 2);
                    tempValues[y * image.Width + x].Alpha = image.Pixels[y * image.Width + x].Alpha;
                }
            }
        });
        return image.ReCreate(image.Width, image.Height, tempValues);
    }
}