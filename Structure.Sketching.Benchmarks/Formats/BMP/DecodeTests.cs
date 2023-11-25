using BenchmarkDotNet.Attributes;
using System.IO;

namespace Structure.Sketching.Benchmarks.Formats.BMP;

public class DecodeTests
{
    [Benchmark(Baseline = true, Description = "FileStream reading")]
    public static void FileStreamReading()
    {
        using var testStream = File.Open("../../../../TestImage/BitmapFilter.bmp", FileMode.Open);
        new Structure.Sketching.Formats.Bmp.BmpFormat().Decode(testStream);
    }

    [Benchmark(Description = "MemoryStream reading")]
    public static void MemoryStreamReading()
    {
        using var testStream = File.Open("../../../../TestImage/BitmapFilter.bmp", FileMode.Open);
        var data = new byte[testStream.Length];
        testStream.Read(data, 0, (int)testStream.Length);
        using var memStream = new MemoryStream(data);
        new Structure.Sketching.Formats.Bmp.BmpFormat().Decode(memStream);
    }
}