using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Structure.Sketching.Tests.Formats.Bmp.Format;

public class FileHeader
{
    [Fact]
    public void CreateByteArray()
    {
        var data = new[] { BitConverter.GetBytes((short)19778), BitConverter.GetBytes((int)1000), BitConverter.GetBytes((int)0), BitConverter.GetBytes((int)54) }.SelectMany(x => x).ToArray();
        var testFileHeader = new Sketching.Formats.Bmp.Format.FileHeader(data);
        Assert.Equal(1000, testFileHeader.FileSize);
        Assert.Equal(54, testFileHeader.Offset);
        Assert.Equal(0, testFileHeader.Reserved);
        Assert.Equal(19778, testFileHeader.Type);
    }

    [Fact]
    public void CreateValues()
    {
        var testFileHeader = new Sketching.Formats.Bmp.Format.FileHeader(1000, 54);
        Assert.Equal(1000, testFileHeader.FileSize);
        Assert.Equal(54, testFileHeader.Offset);
        Assert.Equal(0, testFileHeader.Reserved);
        Assert.Equal(19778, testFileHeader.Type);
    }

    [Fact]
    public void Read()
    {
        var data = new[] { BitConverter.GetBytes((short)19778), BitConverter.GetBytes((int)1000), BitConverter.GetBytes((int)0), BitConverter.GetBytes((int)54) }.SelectMany(x => x).ToArray();
        using var stream = new MemoryStream(data);
        var testFileHeader = Sketching.Formats.Bmp.Format.FileHeader.Read(stream);
        Assert.Equal(1000, testFileHeader.FileSize);
        Assert.Equal(54, testFileHeader.Offset);
        Assert.Equal(0, testFileHeader.Reserved);
        Assert.Equal(19778, testFileHeader.Type);
    }

    [Fact]
    public void Write()
    {
        var testFileHeader = new Sketching.Formats.Bmp.Format.FileHeader(1000, 54);
        using var bWriter = new BinaryWriter(new MemoryStream());
        testFileHeader.Write(bWriter);
        Assert.Equal(14, bWriter.BaseStream.Length);
    }
}