using Kroiko.Domain.Models;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace Kroiko.Client.Pages;

public partial class Converter : IDisposable
{
    [Parameter] public required ConverterContext Context { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        Context.PropertyChanged += OnContextUpdated;
    }
    private void OnContextUpdated(object? sender, PropertyChangedEventArgs e)
    {
        StateHasChanged();
    }
    public void Dispose()
    {
        Context.PropertyChanged -= OnContextUpdated;
    }
}