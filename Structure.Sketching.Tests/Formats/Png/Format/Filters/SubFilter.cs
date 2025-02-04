﻿using Xunit;

namespace Structure.Sketching.Tests.Formats.Png.Format.Filters;

public class SubFilter
{
    [Fact]
    public void Creation()
    {
        var testObject = new Sketching.Formats.Png.Format.Filters.SubFilter();
        Assert.Equal(1, testObject.FilterValue);
    }

    [Fact]
    public void Decode()
    {
        var testObject = new Sketching.Formats.Png.Format.Filters.SubFilter();
        var result = testObject.Decode(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 1);
        Assert.Equal(new byte[] { 1, 3, 6, 10, 15, 21, 28, 36, 45 }, result);
    }

    [Fact]
    public void Encode()
    {
        var testObject = new Sketching.Formats.Png.Format.Filters.SubFilter();
        var result = testObject.Encode(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 1);
        Assert.Equal(new byte[] { 1, 1, 1, 2, 3, 3, 3, 4, 5, 5 }, result);
    }
}