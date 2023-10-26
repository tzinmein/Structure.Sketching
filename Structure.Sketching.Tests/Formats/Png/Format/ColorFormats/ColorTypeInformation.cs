using Xunit;

namespace Structure.Sketching.Tests.Formats.Png.Format.ColorFormats;

public class ColorTypeInformation
{
    [Fact]
    public void Create()
    {
        var TestObject = new Structure.Sketching.Formats.Png.Format.ColorFormats.ColorTypeInformation(1, new[] { 1 }, (x, y) => null);
        Assert.Equal(1, TestObject.ScanlineFactor);
        Assert.Single(TestObject.SupportedBitDepths);
        Assert.Equal(1, TestObject.SupportedBitDepths[0]);
        Assert.Null(TestObject.ScanlineReaderFactory(null, null));
    }
}