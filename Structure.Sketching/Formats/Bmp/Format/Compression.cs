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

namespace Structure.Sketching.Formats.Bmp.Format;

/// <summary>
/// Compression type used by the bitmap file.
/// </summary>
public enum Compression
{
    /// <summary>
    /// Basic RGB bitmap
    /// </summary>
    Rgb = 0,

    /// <summary>
    /// RLE8, only used with 8-BPP bitmaps
    /// </summary>
    Rle8 = 1,

    /// <summary>
    /// RLE4, only used with 4-BPP bitmaps
    /// </summary>
    Rle4 = 2,

    /// <summary>
    /// Uses bitfields to determine pixel layout
    /// </summary>
    Bitfields = 3
}