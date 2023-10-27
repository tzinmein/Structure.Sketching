/*
Copyright 2016 James Craig

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

using Structure.Sketching.Formats.Bmp.Format.PixelFormats.BaseClasses;
using System.Threading.Tasks;

namespace Structure.Sketching.Formats.Bmp.Format.PixelFormats;

/// <summary>
/// RGB 24bit pixel format
/// </summary>
/// <seealso cref="Structure.Sketching.Formats.Bmp.Format.PixelFormats.Interfaces.IPixelFormat"/>
public unsafe class Rgb24Bit : PixelFormatBase
{
    /// <summary>
    /// The bytes per pixel
    /// </summary>
    /// <value>The BPP.</value>
    public override double Bpp => 3;

    /// <summary>
    /// Decodes the specified data.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="data">The data.</param>
    /// <param name="palette">The palette.</param>
    /// <returns>The decoded data</returns>
    public override byte[] Decode(Header header, byte[] data, Palette palette)
    {
        var width = header.Width;
        var height = header.Height;
        var alignment = (4 - width * (int)Bpp % 4) % 4;
        var returnValue = new byte[width * height * 4];
        Parallel.For(0, height, y =>
        {
            var sourceY = height - y - 1;
            if (sourceY < 0)
                sourceY = 0;
            if (sourceY >= height)
                sourceY = height - 1;
            var sourceRowOffset = sourceY * (width * (int)Bpp + alignment);
            var destinationRowOffset = y * width * 4;
            fixed (byte* dataFixed = &data[sourceRowOffset])
            fixed (byte* returnValueFixed = &returnValue[destinationRowOffset])
            {
                var dataFixed2 = dataFixed;
                var returnValueFixed2 = returnValueFixed;
                for (var x = 0; x < width; ++x)
                {
                    *(returnValueFixed2 + 2) = *dataFixed2;
                    ++dataFixed2;
                    *(returnValueFixed2 + 1) = *dataFixed2;
                    ++dataFixed2;
                    *returnValueFixed2 = *dataFixed2;
                    ++dataFixed2;
                    *(returnValueFixed2 + 3) = 255;
                    returnValueFixed2 += 4;
                }
            }
        });
        return returnValue;
    }

    /// <summary>
    /// Encodes the specified data.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="data">The data.</param>
    /// <param name="palette">The palette.</param>
    /// <returns>The encoded data</returns>
    public override byte[] Encode(Header header, byte[] data, Palette palette)
    {
        var width = header.Width;
        var height = header.Height;
        var alignment = (4 - width * (int)Bpp % 4) % 4;
        var returnValue = new byte[(width * (int)Bpp + alignment) * height];
        Parallel.For(0, height, y =>
        {
            var sourceY = height - y - 1;
            if (sourceY < 0)
                sourceY = 0;
            if (sourceY >= height)
                sourceY = height - 1;
            var sourceRowOffset = sourceY * width * 4;
            var destinationRowOffset = y * (width * (int)Bpp + alignment);
            fixed (byte* dataFixed = &data[sourceRowOffset])
            fixed (byte* returnValueFixed = &returnValue[destinationRowOffset])
            {
                var dataFixed2 = dataFixed;
                var returnValueFixed2 = returnValueFixed;
                for (var x = 0; x < width; ++x)
                {
                    *returnValueFixed2 = *(dataFixed2 + 2);
                    ++returnValueFixed2;
                    *returnValueFixed2 = *(dataFixed2 + 1);
                    ++returnValueFixed2;
                    *returnValueFixed2 = *dataFixed2;
                    ++returnValueFixed2;
                    dataFixed2 += 4;
                }
            }
        });
        return returnValue;
    }
}