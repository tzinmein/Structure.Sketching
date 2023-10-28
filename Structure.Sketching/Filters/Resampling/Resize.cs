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
using Structure.Sketching.Filters.Resampling.Enums;
using Structure.Sketching.Filters.Resampling.ResamplingFilters;
using Structure.Sketching.Filters.Resampling.ResamplingFilters.Interfaces;
using Structure.Sketching.Numerics;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Structure.Sketching.Filters.Resampling;

/// <summary>
/// Resizes an image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class Resize : IFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Resize"/> class.
    /// </summary>
    /// <param name="height">The new height.</param>
    /// <param name="width">The new width.</param>
    /// <param name="filter">The filter.</param>
    public Resize(int width, int height, ResamplingFiltersAvailable filter)
    {
        Height = height;
        Width = width;
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
        FilterKey = filter;
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
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the filter key.
    /// </summary>
    /// <value>The filter key.</value>
    private ResamplingFiltersAvailable FilterKey { get; set; }

    /// <summary>
    /// Gets or sets the resampling filter dictionary.
    /// </summary>
    /// <value>The resampling filter.</value>
    private Dictionary<ResamplingFiltersAvailable, IResamplingFilter> ResamplingFilter { get; set; }

    /// <summary>
    /// Applies the resizing filter to the specified image.
    /// </summary>
    /// <param name="image">The image to resize.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The image</returns>
    public Image Apply(Image image, Rectangle targetLocation = default)
    {
        var output = Sample(image);
        image.ReCreate(Width, Height, output);
        return image;
    }

    /// <summary>
    /// Gets the matrix.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The transformation matrix</returns>
    protected Matrix3x2 GetMatrix(Image image, Rectangle targetLocation)
    {
        var xScale = (float)image.Width / Width;
        var yScale = (float)image.Height / Height;
        return Matrix3x2.CreateScale(xScale, yScale);
    }

    private unsafe Color[] Sample(Image image)
    {
        Filter.Precompute(image.Width, image.Height, Width, Height);
        var targetLocation = new Rectangle(0, 0, image.Width, image.Height);
        var output = new Color[Width * Height];
        var transformationMatrix = GetMatrix(image, targetLocation);
        double tempWidth = Width < 0 ? image.Width : Width;
        double tempHeight = Height < 0 ? image.Width : Height;
        var xScale = tempWidth / image.Width;
        var yScale = tempHeight / image.Height;
        var yRadius = yScale < 1f ? Filter.FilterRadius / yScale : Filter.FilterRadius;
        var xRadius = xScale < 1f ? Filter.FilterRadius / xScale : Filter.FilterRadius;

        Parallel.For(0, Height, y =>
        {
            fixed (Color* outputPointer = &output[y * Width])
            {
                var outputPointer2 = outputPointer;
                for (var x = 0; x < Width; ++x)
                {
                    var values = new Vector4(0, 0, 0, 0);
                    float weight = 0;

                    var rotated = Vector2.Transform(new Vector2(x, y), transformationMatrix);
                    var rotatedY = (int)rotated.Y;
                    var rotatedX = (int)rotated.X;
                    var left = (int)(rotatedX - xRadius);
                    var right = (int)(rotatedX + xRadius);
                    var top = (int)(rotatedY - yRadius);
                    var bottom = (int)(rotatedY + yRadius);
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
                        fixed (Color* pixelPointer = &image.Pixels[i * image.Width])
                        {
                            var pixelPointer2 = pixelPointer + left;
                            for (int j = left, xCount = 0; j <= right; ++j, ++xCount)
                            {
                                var tempYWeight = Filter.YWeights[rotatedY].Values[yCount];
                                var tempXWeight = Filter.XWeights[rotatedX].Values[xCount];
                                var tempWeight = tempYWeight * tempXWeight;

                                if (yRadius == 0 && xRadius == 0)
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
        return output;
    }
}