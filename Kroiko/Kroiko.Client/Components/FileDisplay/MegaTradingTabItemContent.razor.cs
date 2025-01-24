using Kroiko.Domain.Models;
using Kroiko.Domain.TemplateBuilding;
using Kroiko.Domain.TemplateBuilding.MegaTrading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.ObjectModel;

namespace Kroiko.Client.Components.FileDisplay;

public partial class MegaTradingTabItemContent
{
    [Parameter] public required KroikoFile File { get; set; }
    [Parameter] public required ConverterContext Context { get; set; }
    
    private ObservableCollection<MegaTradingViewModel> _source;
    private ObservableCollection<MegaTradingGroupModel> _groups = [];
    
    protected override void OnInitialized()
    {
        _source = File.Details.ToMegaTradingViewModel();
        _groups = new(
            _source.GroupBy(x => x.Material)
                .Select(x => new MegaTradingGroupModel(x.Key, x.Key))
                .ToList());
    }
    
    private void OnMaterialsReplaced(MouseEventArgs obj)
    {
        foreach (var detail in _source)
        {
            foreach (var group in _groups.Where(g => g.OldName == detail.Material))
            {
                detail.Material = group.NewName;
            }
        }

        foreach (var group in _groups)
        {
            group.OldName = group.NewName;
        }

        StateHasChanged();

        File.Details = _source.ToKroikoDetails();
    }
}