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

namespace Structure.Sketching.Formats.Jpeg.Format.HelperClasses.Enums;

/// <summary>
/// Huffman index
/// </summary>
public enum HuffmanIndex
{
    /// <summary>
    /// The huffman index luminance dc
    /// </summary>
    HuffmanIndexLuminanceDc = 0,

    /// <summary>
    /// The huffman index luminance ac
    /// </summary>
    HuffmanIndexLuminanceAc = 1,

    /// <summary>
    /// The huffman index chrominance dc
    /// </summary>
    HuffmanIndexChrominanceDc = 2,

    /// <summary>
    /// The huffman index chrominance ac
    /// </summary>
    HuffmanIndexChrominanceAc = 3,
}