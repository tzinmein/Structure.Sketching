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

using Structure.Sketching.Filters.ColorMatrix.BaseClasses;
using Structure.Sketching.Numerics;
using System;

namespace Structure.Sketching.Filters.ColorMatrix;

/// <summary>
/// Hue matrix
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.ColorMatrix.BaseClasses.MatrixBaseClass" />
public class Hue : MatrixBaseClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Alpha"/> class.
    /// </summary>
    /// <param name="value">The angle value (0 to 360).</param>
    public Hue(float value)
    {
        value *= (float)(Math.PI / 180f);
        Value = value;
        var cosRadians = Math.Cos(value);
        var sinRadians = Math.Sin(value);

        const float lumR = .213f;
        const float lumG = .715f;
        const float lumB = .072f;

        const float oneMinusLumR = 1f - lumR;
        const float oneMinusLumG = 1f - lumG;
        const float oneMinusLumB = 1f - lumB;
        Matrix = new Matrix5X5
        {
            M11 = (float)(lumR + cosRadians * oneMinusLumR - sinRadians * lumR),
            M12 = (float)(lumR - cosRadians * lumR - sinRadians * 0.143),
            M13 = (float)(lumR - cosRadians * lumR - sinRadians * oneMinusLumR),
            M21 = (float)(lumG - cosRadians * lumG - sinRadians * lumG),
            M22 = (float)(lumG + cosRadians * oneMinusLumG + sinRadians * 0.140),
            M23 = (float)(lumG - cosRadians * lumG + sinRadians * lumG),
            M31 = (float)(lumB - cosRadians * lumB + sinRadians * oneMinusLumB),
            M32 = (float)(lumB - cosRadians * lumB - sinRadians * 0.283),
            M33 = (float)(lumB + cosRadians * oneMinusLumB + sinRadians * lumB),
            M44 = 1,
            M55 = 1
        };
    }

    /// <summary>
    /// Gets the matrix.
    /// </summary>
    /// <value>The matrix.</value>
    public override Matrix5X5 Matrix { get; } = new(
        1f, 0f, 0f, 0f, 0f,
        0f, 1f, 0f, 0f, 0f,
        0f, 0f, 1f, 0f, 0f,
        0f, 0f, 0f, 1f, 0f,
        0f, 0f, 0f, 0f, 1f
    );

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public float Value { get; }
}