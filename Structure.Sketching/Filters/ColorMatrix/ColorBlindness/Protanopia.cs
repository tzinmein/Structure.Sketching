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

namespace Structure.Sketching.Filters.ColorMatrix.ColorBlindness;

/// <summary>
/// Protanopia matrix
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.ColorMatrix.BaseClasses.MatrixBaseClass" />
public class Protanopia : MatrixBaseClass
{
    /// <summary>
    /// Gets the matrix.
    /// </summary>
    /// <value>The matrix.</value>
    public override Matrix5X5 Matrix => new(
        .567f, .558f, 0f, 0f, 0f,
        .433f, .442f, .242f, 0f, 0f,
        0f, 0f, .758f, 0f, 0f,
        0f, 0f, 0f, 1f, 0f,
        0f, 0f, 0f, 0f, 1f
    );
}