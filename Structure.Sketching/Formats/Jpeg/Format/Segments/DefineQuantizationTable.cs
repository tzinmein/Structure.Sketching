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

using Structure.Sketching.Formats.Jpeg.Format.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Structure.Sketching.Formats.Jpeg.Format.Segments.BaseClasses;

namespace Structure.Sketching.Formats.Jpeg.Format.Segments;

/// <summary>
/// Define Quantization Table Segment
/// </summary>
/// <seealso cref="SegmentBase" />
public class DefineQuantizationTable : SegmentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefineQuantizationTable"/> class.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    public DefineQuantizationTable(ByteBuffer bytes)
        : base(SegmentTypes.DefineQuantizationTable, bytes)
    {
        Quant = new Block[MaximumTq + 1];
        for (var i = 0; i < Quant.Length; i++)
            Quant[i] = new Block();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefineQuantizationTable"/> class.
    /// </summary>
    /// <param name="quality">The quality.</param>
    public DefineQuantizationTable(int quality)
        : base(SegmentTypes.DefineQuantizationTable, null)
    {
        Quant = new Block[MaximumTq - 1];
        for (var i = 0; i < Quant.Length; i++)
        {
            Quant[i] = new Block();
        }

        if (quality < 1) quality = 1;
        if (quality > 100) quality = 100;

        int scale;
        scale = quality < 50 ? 5000 / quality : 200 - quality * 2;
        for (var i = 0; i < Quant.Length; i++)
        {
            for (var j = 0; j < Block.BlockSize; j++)
            {
                int x = _unscaledQuant[i, j];
                x = (x * scale + 50) / 100;
                if (x < 1) x = 1;
                if (x > 255) x = 255;
                Quant[i][j] = (byte)x;
            }
        }
    }

    /// <summary>
    /// Gets the quant.
    /// </summary>
    /// <value>
    /// The quant.
    /// </value>
    public Block[] Quant { get; private set; }

    private const byte MaximumTq = 3;

    /// <summary>
    /// The unscaled quant values
    /// </summary>
    private readonly byte[,] _unscaledQuant =
    {
        // Luminance.
        {
            16, 11, 12, 14, 12, 10, 16, 14,
            13, 14, 18, 17, 16, 19, 24, 40,
            26, 24, 22, 22, 24, 49, 35, 37,
            29, 40, 58, 51, 61, 60, 57, 51,
            56, 55, 64, 72, 92, 78, 64, 68,
            87, 69, 55, 56, 80, 109, 81, 87,
            95, 98, 103, 104, 103, 62, 77, 113,
            121, 112, 100, 120, 92, 101, 103, 99
        },

        // Chrominance.
        {
            17, 18, 18, 24, 21, 24, 47, 26,
            26, 47, 99, 66, 56, 66, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99
        }
    };

    /// <summary>
    /// Setups the specified segments.
    /// </summary>
    /// <param name="segments">The segments.</param>
    /// <exception cref="System.Exception">
    /// bad Tq value
    /// or
    /// bad Pq value
    /// or
    /// DQT has wrong length
    /// </exception>
    public override void Setup(IEnumerable<SegmentBase> segments)
    {
        Length = GetLength(Bytes);
        var n = Length;
        while (n > 0)
        {
            var done = false;

            n--;
            var x = Bytes.ReadByte();
            var tq = (byte)(x & 0x0f);
            if (tq > MaximumTq)
                throw new Exception("bad Tq value");

            switch (x >> 4)
            {
                case 0:
                    if (n < Block.BlockSize)
                    {
                        done = true;
                        break;
                    }
                    n -= Block.BlockSize;
                    Bytes.ReadFull(TempData, 0, Block.BlockSize);

                    for (var i = 0; i < Block.BlockSize; i++)
                        Quant[tq][i] = TempData[i];
                    break;

                case 1:
                    if (n < 2 * Block.BlockSize)
                    {
                        done = true;
                        break;
                    }
                    n -= 2 * Block.BlockSize;
                    Bytes.ReadFull(TempData, 0, 2 * Block.BlockSize);

                    for (var i = 0; i < Block.BlockSize; i++)
                        Quant[tq][i] = (TempData[2 * i] << 8) | TempData[2 * i + 1];
                    break;

                default:
                    throw new Exception("bad Pq value");
            }

            if (done)
                break;
        }

        if (n != 0)
            throw new Exception("DQT has wrong length");
    }

    /// <summary>
    /// Writes the information to the specified writer.
    /// </summary>
    /// <param name="writer">The binary writer.</param>
    public override void Write(BinaryWriter writer)
    {
        Length = 132;
        WriteSegmentHeader(writer);
        for (var i = 0; i < Quant.Length; i++)
        {
            writer.Write((byte)i);
            writer.Write(Quant[i].Data.Select(x => (byte)x).ToArray(), 0, Quant[i].Data.Length);
        }
    }
}