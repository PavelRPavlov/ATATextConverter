using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.Models;
using Kroiko.Domain.TemplateBuilding;
using Kroiko.Domain.TemplateBuilding.Lonira;
using Kroiko.Domain.TemplateBuilding.MegaTrading;
using Kroiko.Domain.TemplateBuilding.Suliver;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Kroiko.Client.Components.FileDisplay;

public partial class FileDisplayComponent : IDisposable
{
    [Parameter] public required ConverterContext Context { get; set; }
    
    private bool _isLoadingFiles = true;
    private bool _isInternalChange = false;
    
    protected override void OnInitialized()
    {
        Context.PropertyChanged += ContextPropertyChanged;
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            LoadDataSource();

            // add 1s delay to show loading indicator
            await Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith((t) =>
            {
                InvokeAsync(() =>
                {
                    _isLoadingFiles = false;
                    StateHasChanged();
                });
            });
        }
    }
    private void ContextPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ConverterContext.Details) ||
            e.PropertyName == nameof(ConverterContext.TargetCompany) &&
            !_isInternalChange)
        {
            _isLoadingFiles = true;
            LoadDataSource();
            _isLoadingFiles = false;
            StateHasChanged();
        }
    }
    private void LoadDataSource()
    {
        var result = new List<KroikoFile>();
        switch (Context.TargetCompany.Name)
        {
            case nameof(SupportedCompanies.Lonira):
                var groups = Context.Details.GroupBy(x => x.Material).ToList();
                foreach (var group in groups.Where(group => !string.IsNullOrEmpty(group.Key)))
                {
                    result.Add(new()
                    {
                        FileName = group.Key,
                        Details = group.ToList().ToLoniraDetails()
                    });
                }

                break;
            case nameof(SupportedCompanies.Suliver):
                if (Context.Details.Any())
                {
                    result.Add(new()
                    {
                        // TODO what is the required file name
                        FileName = "Suliver",
                        Details = Context.Details.ToSuliverDetails()
                    });
                }
                break;
            case nameof(SupportedCompanies.MegaTrading):
                if (Context.Details.Any())
                {
                    result.Add(new()
                    {
                        // TODO what is the required file name
                        FileName = "MegaTrading",
                        Details = Context.Details.ToMegaTradingDetails()
                    });
                }
                break;
        }

        _isInternalChange = true;
        Context.Files = new(result);
        _isInternalChange = false;
    }
    
    public void Dispose()
    {
        Context.PropertyChanged -= ContextPropertyChanged;
    }
    
    private void OnTabCreated(object obj)
    {
        _isLoadingFiles = false;
    }
}