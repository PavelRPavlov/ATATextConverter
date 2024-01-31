using Kroiko.Domain.TemplateBuilding;
using Microsoft.Extensions.Logging;

namespace Kroiko.Domain.ExcelFilesGeneration;

public class FileGeneratorService(ILogger<FileGeneratorService> logger, IExcelFileGenerator excelFileGenerator)
{
    public async Task<List<FileSaveContext>> CreateFiles(ContactInfo contactInfo, IEnumerable<KroikoFile> files, ITemplateBuilder templateBuilder, IFileNameProvider fileNameProvider)
    {
        
        var sheets = await templateBuilder.BuildTemplateAsync(contactInfo, files);
        var fileSaveContexts = await excelFileGenerator.GenerateExcelFilesAsync(sheets, fileNameProvider);
        foreach (var file in fileSaveContexts)
        {
            file.FileName = file.FileName.Replace("{CompanyName}", contactInfo.CompanyName);
        }
        return fileSaveContexts;
    }
}