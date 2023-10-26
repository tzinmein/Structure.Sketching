﻿using Structure.Sketching.Formats.Bmp.Format.PixelFormats.Interfaces;
using Structure.Sketching.Tests.Formats.Bmp.Format.PixelFormats.BaseClasses;
using System.IO;
using Xunit;

namespace Structure.Sketching.Tests.Formats.Bmp.Format.PixelFormats;

public class Rgb4Bit : FormatBaseFixture
{
    public override string FileName => "./TestImages/Formats/Bmp/Test4.bmp";
    public override IPixelFormat Format => new Sketching.Formats.Bmp.Format.PixelFormats.Rgb4Bit();

    [Fact]
    public void Decode()
    {
        using var tempFile = File.Open(FileName, FileMode.Open, FileAccess.Read);
        var data = Format.Read(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 880, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), tempFile);
        data = Format.Decode(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 880, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), data, new Sketching.Formats.Bmp.Format.Palette(16, new byte[64]));
        Assert.Equal(7040, data.Length);
    }

    [Fact]
    public void Encode()
    {
        using var tempFile = File.Open(FileName, FileMode.Open, FileAccess.Read);
        var data = Format.Read(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 880, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), tempFile);
        data = Format.Decode(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 880, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), data, new Sketching.Formats.Bmp.Format.Palette(16, new byte[64]));
        data = Format.Encode(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 880, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), data, new Sketching.Formats.Bmp.Format.Palette(16, new byte[64]));
        Assert.Equal(7040, data.Length);
    }

    [Fact]
    public void Read()
    {
        using var tempFile = File.Open(FileName, FileMode.Open, FileAccess.Read);
        var data = Format.Read(new Sketching.Formats.Bmp.Format.Header(44, 40, 0, 880, 0, 0, 0, 0, Sketching.Formats.Bmp.Format.Compression.Rgb), tempFile);
        Assert.Equal(1760, data.Length);
    }
}