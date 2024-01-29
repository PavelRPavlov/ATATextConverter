using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;
using Microsoft.Extensions.Logging;

namespace Kroiko.Domain.ExcelFilesGeneration;


public class FileGeneratorService(ILogger<FileGeneratorService> logger, IDetailsExtractorService detailsExtractorService, IExcelFileGenerator excelFileGenerator)
{
    
    public async Task<List<FileSaveContext>> CreateFiles(MemoryStream memoryStream, ContactInfo contactInfo, ITemplateBuilder templateBuilder, IFileNameProvider fileNameProvider)
    {
        var details = await detailsExtractorService.ExtractDetails(memoryStream);
        var sheets = await templateBuilder.BuildTemplateAsync(contactInfo, details);;
        var files = await excelFileGenerator.GenerateExcelFilesAsync(sheets, fileNameProvider);
        foreach (var file in files)
        {
            file.FileName = file.FileName.Replace("{CompanyName}", contactInfo.CompanyName);
        }
        return files;
    }
    
    public async Task<List<ISheet>?> CreateSheets(MemoryStream memoryStream, ContactInfo contactInfo, ITemplateBuilder templateBuilder)
    {
        var details = await detailsExtractorService.ExtractDetails(memoryStream);
        var sheets = await templateBuilder.BuildTemplateAsync(contactInfo, details);;

        return sheets as List<ISheet>;
    }
    
    public async Task<List<Detail>> CreateDetails(MemoryStream memoryStream)
    {
        var details = await detailsExtractorService.ExtractDetails(memoryStream);
        return details;
    }
}