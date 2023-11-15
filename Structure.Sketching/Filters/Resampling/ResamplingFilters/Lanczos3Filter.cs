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

using Structure.Sketching.Filters.Resampling.ResamplingFilters.BaseClasses;
using System;

namespace Structure.Sketching.Filters.Resampling.ResamplingFilters;

/// <summary>
/// Lanczos3 filter
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Resampling.ResamplingFilters.Interfaces.IResamplingFilter"/>
public class Lanczos3Filter : ResamplingFilterBase
{
    /// <summary>
    /// Gets the filter radius.
    /// </summary>
    /// <value>The filter radius.</value>
    public override float FilterRadius => 3f;

    /// <summary>
    /// Gets the value based on the resampling filter.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The new value based on the input.</returns>
    public override double GetValue(double value)
    {
        value = Math.Abs(value);
        if (value < 3) return Sin(value) * Sin(value / 3f);
        return 0;
    }

    private double Sin(double value)
    {
        if (value == 0) return 1;
        value *= Math.PI;
        return Math.Sin(value) / value;
    }
}