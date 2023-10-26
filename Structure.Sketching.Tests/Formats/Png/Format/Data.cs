using Xunit;

namespace Structure.Sketching.Tests.Formats.Png.Format;

public class Data
{
    [Fact]
    public void Create()
    {
        var testObject = new Structure.Sketching.Formats.Png.Format.Data(new byte[1000]);
        Assert.Equal(new byte[1000], testObject.ImageData);
        Assert.Equal(1000, testObject.ImageData.Length);
    }

    [Fact]
    public void CreateFromChunk()
    {
        Structure.Sketching.Formats.Png.Format.Data testObject = new Sketching.Formats.Png.Format.Helpers.Chunk(1, "", new byte[1000], 12);
        Assert.Equal(new byte[1000], testObject.ImageData);
        Assert.Equal(1000, testObject.ImageData.Length);
    }
}