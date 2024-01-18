using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ATAFurniture.Server.Services.Template;
using Microsoft.Extensions.Logging;

namespace ATAFurniture.Server.Services.ExcelGenerator;


public class ExcelGeneratorService(ILogger<ExcelGeneratorService> logger, DetailsExtractor detailsExtractor, IExcelFileGenerator excelFileGenerator, ITemplateBuilder templateBuilder)
{
    
    public async Task<List<FileSaveContext>> CreateFiles(MemoryStream memoryStream, ContactInfo contactInfo)
    {
        var details = detailsExtractor.ExtractDetailsAsync(memoryStream);
        var sheets = await templateBuilder.BuildTemplateAsync(contactInfo, details);
        var files = await excelFileGenerator.GenerateExcelFilesAsync(sheets);
        foreach (var file in files)
        {
            file.FileName = file.FileName.Replace("{CompanyName}", contactInfo.CompanyName);
        }
        return files;
    }
}