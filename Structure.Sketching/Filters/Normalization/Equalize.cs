﻿/*
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
using Structure.Sketching.Numerics.Interfaces;
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Normalization;

/// <summary>
/// Equalizes of an image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class Equalize : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Equalize"/> class.
    /// </summary>
    /// <param name="histogram">The histogram.</param>
    public Equalize(IHistogram histogram = null)
    {
        Histogram = histogram ?? new RgbHistogram();
    }

    /// <summary>
    /// Gets or sets the histogram.
    /// </summary>
    /// <value>The histogram.</value>
    private IHistogram Histogram { get; set; }

    /// <summary>
    /// Applies the filter to the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The image</returns>
    public unsafe Image Apply(Image image, Rectangle targetLocation = default)
    {
        targetLocation = targetLocation == default ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
        Histogram.LoadImage(image)
            .Equalize();
        Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
        {
            fixed (Color* targetPointer = &image.Pixels[y * image.Width + targetLocation.Left])
            {
                var targetPointer2 = targetPointer;
                for (var x = targetLocation.Left; x < targetLocation.Right; ++x)
                {
                    var resultColor = Histogram.EqualizeColor(*targetPointer2);
                    (*targetPointer2).Red = resultColor.Red;
                    (*targetPointer2).Green = resultColor.Green;
                    (*targetPointer2).Blue = resultColor.Blue;
                    ++targetPointer2;
                }
            }
        });
        return image;
    }
}