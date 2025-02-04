﻿using Structure.Sketching.Formats.Bmp.Format.PixelFormats.Interfaces;
using Structure.Sketching.Tests.Formats.Bmp.Format.PixelFormats.BaseClasses;
using System.IO;
using Xunit;

namespace Structure.Sketching.Tests.Formats.Bmp.Format.PixelFormats;

public class Rgb8Bit : FormatBaseFixture
{
    public override string FileName => "./TestImages/Formats/Bmp/Test8.bmp";
    public override IPixelFormat Format => new Sketching.Formats.Bmp.Format.PixelFormats.Rgb8Bit();

    [Fact]
    public void Decode()
    {
        var paletteData = new byte[1024];
        var tempPalette = new Sketching.Formats.Bmp.Format.Palette(256, paletteData);
        using var tempFile = File.Open(FileName, FileMode.Open, FileAccess.Read);
        var data = Format.Read(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 1760, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), tempFile);
        data = Format.Decode(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 1760, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), data, tempPalette);
        Assert.Equal(7040, data.Length);
    }

    [Fact(Skip = "Not currently implemented")]
    public void Encode()
    {
        var paletteData = new byte[1024];
        var tempPalette = new Sketching.Formats.Bmp.Format.Palette(256, paletteData);
        using var tempFile = File.Open(FileName, FileMode.Open, FileAccess.Read);
        var data = Format.Read(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 1760, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), tempFile);
        data = Format.Decode(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 1760, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), data, tempPalette);
        data = Format.Encode(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 1760, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), data, tempPalette);
        Assert.Equal(1760, data.Length);
    }

    [Fact]
    public void Read()
    {
        using var tempFile = File.Open(FileName, FileMode.Open, FileAccess.Read);
        var data = Format.Read(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 1760, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), tempFile);
        Assert.Equal(1760, data.Length);
    }
}