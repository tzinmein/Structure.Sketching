using Structure.Sketching.Formats.Png.Format.Enums;
using Xunit;

namespace Structure.Sketching.Tests.Formats.Png.Format;

public class Palette
{
    [Fact]
    public void Create()
    {
        var testObject = new Sketching.Formats.Png.Format.Palette(new byte[512], PaletteType.Color);
        Assert.Equal(512, testObject.Data.Length);
        Assert.Equal(PaletteType.Color, testObject.Type);
        testObject = new Sketching.Formats.Png.Format.Palette(new byte[12], PaletteType.Alpha);
        Assert.Equal(12, testObject.Data.Length);
        Assert.Equal(PaletteType.Alpha, testObject.Type);
    }

    [Fact]
    public void CreateFromChunk()
    {
        Sketching.Formats.Png.Format.Palette testObject = new Sketching.Formats.Png.Format.Helpers.Chunk(1, Sketching.Formats.Png.Format.Helpers.ChunkTypes.Palette, new byte[512], 12);
        Assert.Equal(512, testObject.Data.Length);
        Assert.Equal(PaletteType.Color, testObject.Type);
        testObject = new Sketching.Formats.Png.Format.Helpers.Chunk(1, Sketching.Formats.Png.Format.Helpers.ChunkTypes.TransparencyInfo, new byte[12], 12);
        Assert.Equal(12, testObject.Data.Length);
        Assert.Equal(PaletteType.Alpha, testObject.Type);
    }
}