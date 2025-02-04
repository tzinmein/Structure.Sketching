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

namespace Structure.Sketching.Filters.Resampling.ResamplingFilters.HelperClasses;

/// <summary>
/// Weights holder class
/// </summary>
public class Weights
{
    /// <summary>
    /// Gets or sets the sum.
    /// </summary>
    /// <value>The sum.</value>
    public double TotalWeight { get; set; }

    /// <summary>
    /// The values
    /// </summary>
    public double[] Values;
}