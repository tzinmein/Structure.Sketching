using System;
using BenchmarkDotNet.Attributes;
using Structure.Sketching.Benchmarks.Formats.BMP.TestClasses;
using Structure.Sketching.Formats.Bmp.Format.PixelFormats;

namespace Structure.Sketching.Benchmarks.Formats.BMP;

public class Rgb24Test
{
    [Params(100, 1000, 10000)]
    public int Count { get; set; }

    [Benchmark(Baseline = true, Description = "Without pointers")]
    public void Current()
    {
        new Rgb24Bit().Decode(new Sketching.Formats.Bmp.Format.Header(Count, Count, 0, Count * Count * 3, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), new byte[Count * Count * 3], new Sketching.Formats.Bmp.Format.Palette(0, Array.Empty<byte>()));
    }

    [Benchmark(Description = "With fixed array pointers")]
    public void TestClass()
    {
        new Rgb24BitTest().Decode(new Sketching.Formats.Bmp.Format.Header(Count, Count, 0, Count * Count * 3, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), new byte[Count * Count * 3], new Sketching.Formats.Bmp.Format.Palette(0, Array.Empty<byte>()));
    }
}