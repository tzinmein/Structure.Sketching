﻿using Structure.Sketching.Filters.Interfaces;
using Structure.Sketching.Filters.Morphology;
using Structure.Sketching.Numerics;
using Structure.Sketching.Tests.BaseClasses;
using Xunit;

namespace Structure.Sketching.Tests.Filters;

public class MorphologyFilters : FilterTestBaseClass
{
    public override string ExpectedDirectory => "./ExpectedResults/Filters/";

    public override string OutputDirectory => "./TestOutput/Filters/";

    public static readonly TheoryData<string, IFilter, Rectangle> Filters = new()
    {
        { "Dilate", new Dilate(1),default },
        { "Constrict", new Constrict(1),default },
        { "Dilate-Partial", new Dilate(1),new Rectangle(100,100,500,500) },
        { "Constrict-Partial", new Constrict(1),new Rectangle(100,100,500,500) }
    };

    [Theory]
    [MemberData(nameof(Filters))]
    public void Run(string name, IFilter filter, Rectangle target)
    {
        CheckCorrect(name, filter, target);
    }
}