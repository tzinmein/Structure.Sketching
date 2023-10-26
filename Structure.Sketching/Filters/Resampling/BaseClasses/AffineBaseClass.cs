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
using Structure.Sketching.Filters.Resampling.Enums;
using Structure.Sketching.Filters.Resampling.ResamplingFilters;
using Structure.Sketching.Filters.Resampling.ResamplingFilters.Interfaces;
using Structure.Sketching.Numerics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Resampling.BaseClasses;

/// <summary>
/// Affine transformation base class
/// </summary>
/// <seealso cref="IFilter"/>
public abstract class AffineBaseClass : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AffineBaseClass"/> class.
    /// </summary>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    /// <param name="filter">The filter to use (defaults to nearest neighbor).</param>
    protected AffineBaseClass(int width = -1, int height = -1, ResamplingFiltersAvailable filter = ResamplingFiltersAvailable.NearestNeighbor)
    {
        Width = width;
        Height = height;
        ResamplingFilter = new Dictionary<ResamplingFiltersAvailable, IResamplingFilter>
        {
            { ResamplingFiltersAvailable.Bell, new BellFilter() },
            { ResamplingFiltersAvailable.CatmullRom, new CatmullRomFilter() },
            { ResamplingFiltersAvailable.Cosine, new CosineFilter() },
            { ResamplingFiltersAvailable.CubicBSpline, new CubicBSplineFilter() },
            { ResamplingFiltersAvailable.CubicConvolution, new CubicConvolutionFilter() },
            { ResamplingFiltersAvailable.Hermite, new HermiteFilter() },
            { ResamplingFiltersAvailable.Lanczos3, new Lanczos3Filter() },
            { ResamplingFiltersAvailable.Lanczos8, new Lanczos8Filter() },
            { ResamplingFiltersAvailable.Mitchell, new MitchellFilter() },
            { ResamplingFiltersAvailable.Quadratic, new QuadraticFilter() },
            { ResamplingFiltersAvailable.QuadraticBSpline, new QuadraticBSplineFilter() },
            { ResamplingFiltersAvailable.Triangle, new TriangleFilter() },
            { ResamplingFiltersAvailable.Bilinear, new BilinearFilter() },
            { ResamplingFiltersAvailable.NearestNeighbor, new NearestNeighborFilter() },
            { ResamplingFiltersAvailable.Robidoux, new RobidouxFilter() },
            { ResamplingFiltersAvailable.RobidouxSharp, new RobidouxSharpFilter() },
            { ResamplingFiltersAvailable.RobidouxSoft, new RobidouxSoftFilter() },
            { ResamplingFiltersAvailable.Bicubic, new BicubicFilter() }
        };
        Filter = ResamplingFilter[filter];
    }

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    /// <value>The filter.</value>
    public IResamplingFilter Filter { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>The height.</value>
    protected int Height { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    protected int Width { get; set; }

    /// <summary>
    /// Gets or sets the resampling filter dictionary.
    /// </summary>
    /// <value>The resampling filter.</value>
    private Dictionary<ResamplingFiltersAvailable, IResamplingFilter> ResamplingFilter { get; set; }

    /// <summary>
    /// Gets the transformation matrix.
    /// </summary>
    /// <value>The transformation matrix.</value>
    private Matrix3x2 TransformationMatrix { get; set; }

    /// <summary>
    /// Gets or sets the x radius for the sampling filter.
    /// </summary>
    /// <value>The x radius.</value>
    private double XRadius { get; set; }

    /// <summary>
    /// Gets or sets the y radius for the sampling filter.
    /// </summary>
    /// <value>The y radius.</value>
    private double YRadius { get; set; }

    /// <summary>
    /// Applies the filter to the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The image</returns>
    public unsafe Image Apply(Image image, Rectangle targetLocation = default)
    {
        Filter.Precompute(image.Width, image.Height, Width, Height);
        targetLocation = targetLocation == default ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
        var copy = new Color[image.Pixels.Length];
        Array.Copy(image.Pixels, copy, copy.Length);
        TransformationMatrix = GetMatrix(image, targetLocation);
        double tempWidth = Width < 0 ? image.Width : Width;
        double tempHeight = Height < 0 ? image.Width : Height;
        var xScale = tempWidth / image.Width;
        var yScale = tempHeight / image.Height;
        YRadius = yScale < 1f ? Filter.FilterRadius / yScale : Filter.FilterRadius;
        XRadius = xScale < 1f ? Filter.FilterRadius / xScale : Filter.FilterRadius;

        Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
        {
            fixed (Color* outputPointer = &image.Pixels[y * image.Width + targetLocation.Left])
            {
                var outputPointer2 = outputPointer;
                for (var x = targetLocation.Left; x < targetLocation.Right; ++x)
                {
                    var values = new Vector4(0, 0, 0, 0);
                    float weight = 0;

                    var rotated = Vector2.Transform(new Vector2(x, y), TransformationMatrix);
                    var rotatedY = (int)rotated.Y;
                    var rotatedX = (int)rotated.X;
                    if (rotatedY >= image.Height
                        || rotatedY < 0
                        || rotatedX >= image.Width
                        || rotatedX < 0)
                    {
                        (*outputPointer2).Red = 0;
                        (*outputPointer2).Green = 0;
                        (*outputPointer2).Blue = 0;
                        (*outputPointer2).Alpha = 255;
                        ++outputPointer2;
                        continue;
                    }
                    var left = (int)(rotatedX - XRadius);
                    var right = (int)(rotatedX + XRadius);
                    var top = (int)(rotatedY - YRadius);
                    var bottom = (int)(rotatedY + YRadius);
                    if (top < 0)
                        top = 0;
                    if (bottom >= image.Height)
                        bottom = image.Height - 1;
                    if (left < 0)
                        left = 0;
                    if (right >= image.Width)
                        right = image.Width - 1;
                    for (int i = top, yCount = 0; i <= bottom; ++i, ++yCount)
                    {
                        fixed (Color* pixelPointer = &copy[i * image.Width])
                        {
                            var pixelPointer2 = pixelPointer + left;
                            for (int j = left, xCount = 0; j <= right; ++j, ++xCount)
                            {
                                var tempYWeight = Filter.YWeights[rotatedY].Values[yCount];
                                var tempXWeight = Filter.XWeights[rotatedX].Values[xCount];
                                var tempWeight = tempYWeight * tempXWeight;

                                if (YRadius == 0 && XRadius == 0)
                                    tempWeight = 1;

                                if (tempWeight == 0)
                                {
                                    ++pixelPointer2;
                                    continue;
                                }
                                values.X += (*pixelPointer2).Red * (float)tempWeight;
                                values.Y += (*pixelPointer2).Green * (float)tempWeight;
                                values.Z += (*pixelPointer2).Blue * (float)tempWeight;
                                values.W += (*pixelPointer2).Alpha * (float)tempWeight;
                                ++pixelPointer2;
                                weight += (float)tempWeight;
                            }
                        }
                    }
                    if (weight == 0)
                        weight = 1;
                    if (weight > 0)
                    {
                        values = Vector4.Clamp(values, Vector4.Zero, new Vector4(255, 255, 255, 255));
                        (*outputPointer2).Red = (byte)values.X;
                        (*outputPointer2).Green = (byte)values.Y;
                        (*outputPointer2).Blue = (byte)values.Z;
                        (*outputPointer2).Alpha = (byte)values.W;
                        ++outputPointer2;
                    }
                    else
                        ++outputPointer2;
                }
            }
        });
        return image;
    }

    /// <summary>
    /// Gets the matrix.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The matrix used for the transformation</returns>
    protected abstract Matrix3x2 GetMatrix(Image image, Rectangle targetLocation);
}