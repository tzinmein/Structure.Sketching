using Structure.Sketching.Colors;
using Structure.Sketching.Filters.Interfaces;
using Structure.Sketching.Numerics;
using System.Threading.Tasks;

namespace Structure.Sketching.Benchmarks.Filters.TestClasses;

/// <summary>
/// Crops the image
/// </summary>
/// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
public class CropFilter : IFilter
{
    /// <summary>
    /// Applies the filter to the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="targetLocation">The target location.</param>
    /// <returns>The image</returns>
    public unsafe Image Apply(Image image, Rectangle targetLocation = default)
    {
        targetLocation = targetLocation == default ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
        var result = new Color[targetLocation.Width * targetLocation.Height];
        Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
        {
            fixed (Color* sourcePointer = image.Pixels)
            {
                fixed (Color* targetPointer = result)
                {
                    var targetPointer2 = targetPointer + (y - targetLocation.Bottom) * targetLocation.Width;
                    var sourcePointer2 = sourcePointer + (y * image.Width + targetLocation.Left);
                    for (var x = targetLocation.Left; x < targetLocation.Right; ++x)
                    {
                        *targetPointer2++ = *sourcePointer2++;
                    }
                }
            }
        });
        return image.ReCreate(targetLocation.Width, targetLocation.Height, result);
    }
}