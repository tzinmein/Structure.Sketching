using Xunit;

namespace Structure.Sketching.Tests.Numerics;

public class RandomTests
{
    [Fact]
    public void ThreadSafeNext()
    {
        Assert.InRange(Sketching.Numerics.Random.ThreadSafeNext(-10, 10), -10, 10);
    }

    [Fact]
    public void ThreadSafeNextDecimal()
    {
        Assert.InRange(Sketching.Numerics.Random.ThreadSafeNextDouble(-10, 10), -10, 10);
    }
}