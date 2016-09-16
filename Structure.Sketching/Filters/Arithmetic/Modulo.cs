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

using Structure.Sketching.Filters.Interfaces;
using Structure.Sketching.Numerics;
using System.Numerics;
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Arithmetic
{
    /// <summary>
    /// Does a modulo operation between two images.
    /// </summary>
    /// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter" />
    public class Modulo : IFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Modulo"/> class.
        /// </summary>
        /// <param name="secondImage">The second image.</param>
        public Modulo(Image secondImage)
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
        public unsafe Image Apply(Image image, Rectangle targetLocation = default(Rectangle))
        {
            targetLocation = targetLocation == default(Rectangle) ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
            Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
            {
                if (y >= SecondImage.Height)
                    return;
                fixed (Vector4* Pointer = &image.Pixels[(y * image.Width) + targetLocation.Left])
                {
                    Vector4* OutputPointer = Pointer;
                    fixed (Vector4* Image2Pointer = &SecondImage.Pixels[(y - targetLocation.Bottom) * SecondImage.Width])
                    {
                        Vector4* Image2Pointer2 = Image2Pointer;
                        int x2 = 0;
                        for (int x = targetLocation.Left; x < targetLocation.Right; ++x)
                        {
                            if (x2 > SecondImage.Width)
                                break;
                            ++x2;
                            *OutputPointer = Vector4.Clamp(new Vector4((*OutputPointer).X % (*Image2Pointer2).X, (*OutputPointer).Y % (*Image2Pointer2).Y, (*OutputPointer).Z % (*Image2Pointer2).Z, (*OutputPointer).W % (*Image2Pointer2).W), Vector4.Zero, Vector4.One);
                            ++OutputPointer;
                            ++Image2Pointer2;
                        }
                    }
                }
            });
            return image;
        }
    }
}