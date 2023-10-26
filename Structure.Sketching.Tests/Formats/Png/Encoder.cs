using Structure.Sketching.Tests.Formats.BaseClasses;
using System.IO;
using Xunit;

namespace Structure.Sketching.Tests.Formats.Png;

public class Encoder : FormatTestBase
{
    public override string ExpectedDirectory => "./ExpectedResults/Formats/Png/";

    public override string InputDirectory => "./TestImages/Formats/Png/";

    public override string OutputDirectory => "./TestOutput/Formats/Png/Encoder/";

    public static readonly TheoryData<string> InputFileNames =
        new()
        {
            "splash.png",
            "48bit.png",
            "blur.png",
            "indexed.png",
            "splashbw.png"
        };

    [Fact]
    public void CanEncode()
    {
        Assert.True(new Sketching.Formats.Png.Encoder().CanEncode("ASDF.png"));
        Assert.False(new Sketching.Formats.Png.Encoder().CanEncode("ASDF.bmp"));
        Assert.False(new Sketching.Formats.Png.Encoder().CanEncode("ASDF.jpg"));
        Assert.False(new Sketching.Formats.Png.Encoder().CanEncode("bmp.gif"));
    }

    [Theory]
    [MemberData(nameof(InputFileNames))]
    public void Encode(string fileName)
    {
        using (var inputFile = File.OpenRead(InputDirectory + fileName))
        {
            var decoder = new Sketching.Formats.Png.Decoder();
            var image = decoder.Decode(inputFile);
            var encoder = new Sketching.Formats.Png.Encoder();
            using var outputFile = File.OpenWrite(OutputDirectory + fileName);
            encoder.Encode(new BinaryWriter(outputFile), image);
        }

        Assert.True(CheckDecodedPngCorrect(fileName));
    }
}