using FluentAssertions;
using Kroiko.Domain.TemplateBuilding;
using Xunit;

namespace ATAFurniture.Server.Tests.TemplateServiceTests;

public class LoniraTemplateBuilderTests
{
    [Theory]
    [InlineData("A1",1,1)]
    [InlineData("B1",1,2)]
    [InlineData("A2",2,1)]
    [InlineData("B2",2,2)]
    [InlineData("C9",9,3)]
    [InlineData("E10",10,5)]
    [InlineData("Z99",99,26)]
    public async Task GivenValidCellNames_WhenConvertingCompoundToNumeric_ThenReturnsCorrectNumbers(string input, int row, int col)
    {
        var result = Cell.GetRowAndColumn(new Cell(input));
        result.Should().Be((row, col));
    }

    [Theory]
    [InlineData(1,1, "A1")]
    [InlineData(2,1, "A2")]
    [InlineData(1,2, "B1")]
    [InlineData(2,2, "B2")]
    [InlineData(11,3, "C11")]
    [InlineData(21,4, "D21")]
    [InlineData(99,26, "Z99")]
    public async Task GivenValidCell_WhenConvertingToStringName_ThenReturnsCorrectCellName(int row, int col,
        string expected)
    {
        var result = Cell.GetCellName(row, col);
        result.Should().Be(expected);
    }
}