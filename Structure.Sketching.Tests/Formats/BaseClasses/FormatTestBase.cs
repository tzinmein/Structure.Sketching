using System.IO;
using System.Linq;

namespace Structure.Sketching.Tests.Formats.BaseClasses
{
    public abstract class FormatTestBase
    {
        protected FormatTestBase()
        {
            new DirectoryInfo(OutputDirectory).Create();
        }

        public abstract string ExpectedDirectory { get; }

        public abstract string InputDirectory { get; }
        public abstract string OutputDirectory { get; }

        protected bool CheckFileCorrect(string fileName)
        {
            using FileStream OutputStream = File.OpenRead(OutputDirectory + fileName);
            using FileStream ExpectedStream = File.OpenRead(ExpectedDirectory + fileName);
            return ReadBinary(OutputStream).SequenceEqual(ReadBinary(ExpectedStream));
        }


        protected bool CheckDecodedPngCorrect(string fileName)
        {
            using FileStream expectedStream = File.OpenRead(ExpectedDirectory + fileName);
            using FileStream outputStream = File.OpenRead(OutputDirectory + fileName);

            var ImageFormat = new Structure.Sketching.Formats.Png.PngFormat();
            var expectedImage = ImageFormat.Decode(expectedStream);
            var outputImage = ImageFormat.Decode(outputStream);

            var dimensionsOk = outputImage.Width == expectedImage.Width && outputImage.Height == expectedImage.Height;
            if (!dimensionsOk) return false;

            var pixelsOk = true;
            for (var index = 0; index < outputImage.Pixels.Length; index++)
            {
                if (!outputImage.Pixels[index].Equals(expectedImage.Pixels[index]))
                    pixelsOk = false;
            }

            return pixelsOk;
        }

        protected byte[] ReadBinary(FileStream stream)
        {
            byte[] Buffer = new byte[1024];
            using MemoryStream Temp = new();
            while (true)
            {
                var Count = stream.Read(Buffer, 0, Buffer.Length);
                if (Count <= 0)
                    return Temp.ToArray();
                Temp.Write(Buffer, 0, Count);
            }
        }
    }
}