using Kroiko.Domain.Models;
using Kroiko.Domain.TemplateBuilding;
using Kroiko.Domain.TemplateBuilding.Suliver;
using Microsoft.AspNetCore.Components;

namespace Kroiko.Client.Components.FileDisplay;

public partial class SuliverTabItemContent
{
    [Parameter] public KroikoFile File { get; set; }
    [Parameter] public ConverterContext Context { get; set; }
    private List<SuliverDetail> _source;
    private bool _isDifferentEdgeColorNotePresent = false;
    protected override void OnInitialized()
    {
        _source = File.Details.Cast<SuliverDetail>().ToList();
        _isDifferentEdgeColorNotePresent = _source.Any(x => x.IsEdgeColorDifferent);
    }
}