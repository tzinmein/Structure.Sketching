﻿using Xunit;

namespace Structure.Sketching.Tests.Formats.Png.Format.Filters;

public class NoFilter
{
    [Fact]
    public void Creation()
    {
        var testObject = new Sketching.Formats.Png.Format.Filters.NoFilter();
        Assert.Equal(0, testObject.FilterValue);
    }

    [Fact]
    public void Decode()
    {
        var testObject = new Sketching.Formats.Png.Format.Filters.NoFilter();
        var result = testObject.Decode(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 1);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, result);
    }

    [Fact]
    public void Encode()
    {
        var testObject = new Sketching.Formats.Png.Format.Filters.NoFilter();
        var result = testObject.Encode(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 1);
        Assert.Equal(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, result);
    }
}