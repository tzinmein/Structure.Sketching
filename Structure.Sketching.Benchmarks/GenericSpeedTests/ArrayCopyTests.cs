using BenchmarkDotNet.Attributes;
using System;
using System.Runtime.CompilerServices;

namespace Structure.Sketching.Benchmarks.GenericSpeedTests;

public class ArrayCopyTests
{
    [Params(100, 1000, 10000)]
    public int Count { get; set; }

    private byte[] _source, _destination;

    [Benchmark(Baseline = true, Description = "Copy using Array.Copy()")]
    public void CopyArray()
    {
        Array.Copy(_source, _destination, Count);
    }

    [Benchmark(Description = "Copy using Unsafe<T>")]
    public unsafe void CopyUnsafe()
    {
        fixed (byte* pinnedDestination = _destination)
        fixed (byte* pinnedSource = _source)
        {
            Unsafe.CopyBlock(pinnedSource, pinnedDestination, (uint)Count);
        }
    }

    [Benchmark(Description = "Copy using Buffer.MemoryCopy<T>")]
    public unsafe void CopyUsingBufferMemoryCopy()
    {
        fixed (byte* pinnedDestination = _destination)
        fixed (byte* pinnedSource = _source)
        {
            Buffer.MemoryCopy(pinnedSource, pinnedDestination, Count, Count);
        }
    }

    [IterationSetup]
    public void SetUp()
    {
        _source = new byte[Count];
        _destination = new byte[Count];
    }
}