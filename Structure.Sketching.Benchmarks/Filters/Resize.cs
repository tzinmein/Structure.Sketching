using BenchmarkDotNet.Attributes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace Structure.Sketching.Benchmarks.Filters;

public class Resize
{
    [Benchmark(Description = "Structure.Sketching Resize")]
    public void CropStructureSketching()
    {
        var testImage = new Sketching.Image(2000, 2000);
        var filter = new Sketching.Filters.Resampling.Resize(400, 400, Sketching.Filters.Resampling.Enums.ResamplingFiltersAvailable.NearestNeighbor);
        filter.Apply(testImage);
    }

    [SupportedOSPlatform("windows")]
    [Benchmark(Baseline = true, Description = "System.Drawing Resize")]
    public void ResizeSystemDrawing()
    {
        using Bitmap source = new(2000, 2000);
        using Bitmap destination = new(400, 400);
        using var graphics = Graphics.FromImage(destination);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.DrawImage(source, 0, 0, 400, 400);
    }
}