using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;
using Microsoft.Extensions.Logging;

namespace Kroiko.Domain.ExcelFilesGeneration;


public class FileGeneratorService(ILogger<FileGeneratorService> logger, IDetailsExtractorService detailsExtractorService, IExcelFileGenerator excelFileGenerator)
{
    
    public async Task<List<FileSaveContext>> CreateFiles(MemoryStream memoryStream, ContactInfo contactInfo, ITemplateBuilder templateBuilder, IFileNameProvider fileNameProvider)
    {
        var details = detailsExtractorService.ExtractDetails(memoryStream);
        if (details is null)
        {
            return null;
        }
        var sheets = await templateBuilder.BuildTemplateAsync(contactInfo, details);;
        var files = await excelFileGenerator.GenerateExcelFilesAsync(sheets, fileNameProvider);
        foreach (var file in files)
        {
            file.FileName = file.FileName.Replace("{CompanyName}", contactInfo.CompanyName);
        }
        return files;
    }
}