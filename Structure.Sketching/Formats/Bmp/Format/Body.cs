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

using Structure.Sketching.Formats.Bmp.Format.PixelFormats;
using Structure.Sketching.Formats.Bmp.Format.PixelFormats.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Structure.Sketching.Formats.Bmp.Format;

/// <summary>
/// BMP body
/// </summary>
public class Body
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Body"/> class.
    /// </summary>
    /// <param name="data">The data.</param>
    public Body(byte[] data)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Body" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="header">The header.</param>
    public Body(Image image, Header header)
        : this(new Rgb24Bit().Encode(header, image.Pixels.SelectMany(x => (byte[])x).ToArray(), null))
    {
    }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>The data.</value>
    public byte[] Data { get; set; }

    /// <summary>
    /// The pixel formats
    /// </summary>
    private static readonly Dictionary<int, IPixelFormat> PixelFormats = new()
    {
        [32] = new Rgb32Bit(),
        [24] = new Rgb24Bit(),
        [16] = new Rgb16Bit(),
        [8] = new Rgb8Bit(),
        [4] = new Rgb4Bit(),
        [1] = new Rgb1Bit()
    };

    /// <summary>
    /// Reads the specified stream.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="palette">The palette.</param>
    /// <param name="stream">The stream.</param>
    /// <returns>The resulting Body information</returns>
    public static Body Read(Header header, Palette palette, Stream stream)
    {
        var data = PixelFormats[header.Bpp].Read(header, stream);
        var data2 = PixelFormats[header.Bpp].Decode(header, data, palette);
        return new Body(data2);
    }

    /// <summary>
    /// Writes to the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public void Write(BinaryWriter writer)
    {
        var amount = Data.Length * 3 % 4;
        if (amount != 0)
        {
            amount = 4 - amount;
        }
        writer.Write(Data, 0, Data.Length);
        for (var x = 0; x < amount; ++x)
        {
            writer.Write((byte)0);
        }
    }
}