using Kroiko.Domain;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Kroiko.Client.Components;

public partial class FileUploadComponent
{
    [Inject] private IDetailsExtractor DetailsExtractor { get; set; }
    [Parameter] public required ConverterContext Context { get; set; }
    
    private bool _isLoadingFiles;
    private bool _isError;
    
    private async Task OnClientChange(UploadChangeEventArgs arg)
    {
        _isError = false;
        _isLoadingFiles = true;

        var stream = arg.Files.First().Source.OpenReadStream();
        var rawText = await new StreamReader(stream).ReadToEndAsync();
        var details = await DetailsExtractor.ExtractDetails(rawText);
        
        _isLoadingFiles = false;
        if (details.Count == 0)
        {
            _isError = true;
            return;
        }

        Context.Details = new(details);
    }
}