using Structure.Sketching.Filters.Interfaces;
using Structure.Sketching.Filters.Normalization;
using Structure.Sketching.Numerics;
using Structure.Sketching.Tests.BaseClasses;
using Xunit;

namespace Structure.Sketching.Tests.Filters
{
    public class NormalizationFilters : FilterTestBaseClass
    {
        public override string ExpectedDirectory => "./ExpectedResults/Filters/";

        public override string OutputDirectory => "./TestOutput/Filters/";

        public static readonly TheoryData<string, IFilter, Rectangle> Filters = new TheoryData<string, IFilter, Rectangle>
        {
            { "Logarithm", new Logarithm(),default },
            { "Gamma", new Gamma(.2f),default },
            { "AdaptiveEqualize", new AdaptiveEqualize(5),default },
            { "AdaptiveHSVEqualize", new AdaptiveEqualize(5,()=>new HSVHistogram()),default },
            { "HSVEqualize", new Equalize(new HSVHistogram()),default },
            { "Equalize", new Equalize(),default },
            { "StretchContrast", new StretchContrast(),default },

            { "Logarithm-Partial", new Logarithm(),new Rectangle(100,100,500,500) },
            { "Gamma-Partial", new Gamma(.2f),new Rectangle(100,100,500,500) },
            { "AdaptiveEqualize-Partial", new AdaptiveEqualize(5),new Rectangle(100,100,500,500) },
            { "AdaptiveHSVEqualize-Partial", new AdaptiveEqualize(5,()=>new HSVHistogram()),new Rectangle(100,100,500,500) },
            { "HSVEqualize-Partial", new Equalize(new HSVHistogram()),new Rectangle(100,100,500,500) },
            { "Equalize-Partial", new Equalize(),new Rectangle(100,100,500,500) },
            { "StretchContrast-Partial", new StretchContrast(),new Rectangle(100,100,500,500) }
        };

        [Theory]
        [MemberData(nameof(Filters))]
        public void Run(string name, IFilter filter, Rectangle target)
        {
            CheckCorrect(name, filter, target);
        }
    }
}