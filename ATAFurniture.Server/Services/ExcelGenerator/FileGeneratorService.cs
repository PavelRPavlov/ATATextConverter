using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ATAFurniture.Server.Models;
using ATAFurniture.Server.Services.DetailsExtractor;
using ATAFurniture.Server.Services.Template;
using Microsoft.Extensions.Logging;

namespace ATAFurniture.Server.Services.ExcelGenerator;


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