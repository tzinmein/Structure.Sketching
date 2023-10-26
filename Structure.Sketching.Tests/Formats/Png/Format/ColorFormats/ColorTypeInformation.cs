﻿using Xunit;

namespace Structure.Sketching.Tests.Formats.Png.Format.ColorFormats;

public class ColorTypeInformation
{
    [Fact]
    public void Create()
    {
        var testObject = new Structure.Sketching.Formats.Png.Format.ColorFormats.ColorTypeInformation(1, new[] { 1 }, (x, y) => null);
        Assert.Equal(1, testObject.ScanlineFactor);
        Assert.Single(testObject.SupportedBitDepths);
        Assert.Equal(1, testObject.SupportedBitDepths[0]);
        Assert.Null(testObject.ScanlineReaderFactory(null, null));
    }
}