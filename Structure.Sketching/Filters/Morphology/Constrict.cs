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

namespace Structure.Sketching.Filters.Morphology;

/// <summary>
/// Constricts an image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class Constrict : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Constrict"/> class.
    /// </summary>
    /// <param name="apetureRadius">The apeture radius.</param>
    public Constrict(int apetureRadius)
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
            fixed (Color* targetPointer = &image.Pixels[y * image.Width + targetLocation.Left])
            {
                var targetPointer2 = targetPointer;
                for (var x = targetLocation.Left; x < targetLocation.Right; ++x)
                {
                    var rValue = byte.MaxValue;
                    var gValue = byte.MaxValue;
                    var bValue = byte.MaxValue;
                    for (var y2 = apetureMin; y2 < apetureMax; ++y2)
                    {
                        var tempY = y + y2;
                        var tempX = x + apetureMin;
                        if (tempY >= 0 && tempY < image.Height)
                        {
                            var length = ApetureRadius * 2;
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
                                    rValue = rValue < tempR ? rValue : tempR;
                                    gValue = gValue < tempG ? gValue : tempG;
                                    bValue = bValue < tempB ? bValue : tempB;
                                    ++tempX;
                                }
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