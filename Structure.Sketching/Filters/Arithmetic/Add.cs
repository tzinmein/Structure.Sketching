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
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Arithmetic;

/// <summary>
/// Does an add operation between two images.
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class Add : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Add"/> class.
    /// </summary>
    /// <param name="secondImage">The second image.</param>
    public Add(Image secondImage)
    {
        SecondImage = secondImage;
    }

    /// <summary>
    /// Gets or sets the second image.
    /// </summary>
    /// <value>The second image.</value>
    public Image SecondImage { get; set; }

    /// <summary>
    /// Applies the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The image</returns>
    public unsafe Image Apply(Image image, Rectangle targetLocation = default)
    {
        targetLocation = targetLocation == default ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
        Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
        {
            if (y >= SecondImage.Height)
            {
                return;
            }
            fixed (Color* pointer = &image.Pixels[y * image.Width + targetLocation.Left])
            {
                var outputPointer = pointer;
                fixed (Color* image2Pointer = &SecondImage.Pixels[(y - targetLocation.Bottom) * SecondImage.Width])
                {
                    var image2Pointer2 = image2Pointer;
                    var x2 = 0;
                    for (var x = targetLocation.Left; x < targetLocation.Right; ++x)
                    {
                        if (x2 > SecondImage.Width)
                        {
                            break;
                        }
                        ++x2;
                        (*outputPointer).Add(*image2Pointer2);
                        ++outputPointer;
                        ++image2Pointer2;
                    }
                }
            }
        });
        return image;
    }
}