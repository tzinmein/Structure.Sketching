using Structure.Sketching.Formats.Bmp.Format.PixelFormats.Interfaces;
using Structure.Sketching.Tests.Formats.Bmp.Format.PixelFormats.BaseClasses;
using System.IO;
using Xunit;

namespace Structure.Sketching.Tests.Formats.Bmp.Format.PixelFormats;

public class Rgb24Bit : FormatBaseFixture
{
    public override string FileName => "./TestImages/Formats/Bmp/Test24.bmp";
    public override IPixelFormat Format => new Sketching.Formats.Bmp.Format.PixelFormats.Rgb24Bit();

    [Fact]
    public void Decode()
    {
        using var tempFile = File.Open(FileName, FileMode.Open, FileAccess.Read);
        var data = Format.Read(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 5280, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), tempFile);
        data = Format.Decode(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 5280, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), data, null);
        Assert.Equal(7040, data.Length);
    }

    [Fact]
    public void Encode()
    {
        using var tempFile = File.Open(FileName, FileMode.Open, FileAccess.Read);
        var data = Format.Read(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 5280, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), tempFile);
        data = Format.Decode(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 5280, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), data, null);
        data = Format.Encode(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 5280, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), data, null);
        Assert.Equal(5280, data.Length);
    }

    [Fact]
    public void Read()
    {
        using var tempFile = File.Open(FileName, FileMode.Open, FileAccess.Read);
        var data = Format.Read(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 5280, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), tempFile);
        Assert.Equal(5280, data.Length);
    }
}