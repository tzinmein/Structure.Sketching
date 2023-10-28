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

using Structure.Sketching.Filters.Convolution.BaseClasses;

namespace Structure.Sketching.Filters.Convolution;

/// <summary>
/// Generic convolution filter
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Convolution.BaseClasses.ConvolutionBaseClass" />
public class ConvolutionFilter : ConvolutionBaseClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConvolutionFilter"/> class.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="absolute">if set to <c>true</c> [absolute].</param>
    /// <param name="offset">The offset.</param>
    public ConvolutionFilter(float[] matrix, int width, int height, bool absolute, float offset)
    {
        _matrix = matrix;
        _width = width;
        _height = height;
        _absolute = absolute;
        _offset = offset;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="ConvolutionBaseClass"/> is absolute.
    /// </summary>
    /// <value><c>true</c> if absolute; otherwise, <c>false</c>.</value>
    public override bool Absolute => _absolute;

    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <value>The height.</value>
    public override int Height => _height;

    /// <summary>
    /// Gets the matrix.
    /// </summary>
    /// <value>The matrix.</value>
    public override float[] Matrix => _matrix;

    /// <summary>
    /// Gets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public override float Offset => _offset;

    /// <summary>
    /// Gets the width.
    /// </summary>
    /// <value>The width.</value>
    public override int Width => _width;

    private readonly bool _absolute;
    private readonly int _height;
    private readonly float[] _matrix;
    private readonly float _offset;
    private readonly int _width;
}