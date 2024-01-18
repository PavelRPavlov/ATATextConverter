using ATAFurniture.Server.Services.ExcelGenerator;
using ATAFurniture.Server.Services.Template.Lonira;
using FluentAssertions;
using Snapshooter.Xunit;
using Xunit;

namespace ATAFurniture.Server.Tests.TemplateServiceTests;

public class LoniraTableRowProviderTests
{
    [Fact]
    public async Task GivenDetailWithFullEdges_WhenCreatingTableRows_ThenReturnsCorrectRows()
    {
        var provider = new LoniraTableRowProvider();
        var detail = new Detail(50, 100, 2, "Material", false, true, true, true, true, "Cabinet", 1);
        
        var result = provider.GetTableRow(detail, 1, 1);
        
        result.Should().MatchSnapshot();
    }
    
    [Fact]
    public async Task GivenDetailWithOneLongEdge_WhenCreatingTableRows_ThenReturnsCorrectRows()
    {
        var provider = new LoniraTableRowProvider();
        var detail = new Detail(50, 100, 2, "Material", false, false, true, false, false, "Cabinet", 1);
        
        var result = provider.GetTableRow(detail, 1, 1);
        
        result.Should().MatchSnapshot();
    }
    
    [Fact]
    public async Task GivenDetailWithOneLongEdgeAndRotatedGrain_WhenCreatingTableRows_ThenReturnsCorrectRows()
    {
        var provider = new LoniraTableRowProvider();
        var detail = new Detail(50, 100, 2, "Material", true, false, true, false, false, "Cabinet", 1);
        
        var result = provider.GetTableRow(detail, 1, 1);
        
        result.Should().MatchSnapshot();
    }
}