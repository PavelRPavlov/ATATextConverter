using Kroiko.Domain.Models;
using Kroiko.Domain.TemplateBuilding;
using Kroiko.Domain.TemplateBuilding.Lonira;
using Microsoft.AspNetCore.Components;

namespace Kroiko.Client.Components.FileDisplay;

public partial class LoniraTabItemContent
{
    [Parameter] public KroikoFile File { get; set; }
    [Parameter] public ConverterContext Context { get; set; }
    private List<LoniraDetail> _source;
    protected override void OnInitialized()
    {
        _source = File.Details.Cast<LoniraDetail>().ToList();
    }
}