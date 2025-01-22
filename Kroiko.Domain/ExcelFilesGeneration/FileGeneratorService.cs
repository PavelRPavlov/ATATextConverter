using Kroiko.Domain.ExcelFilesGeneration.XlsxWrapper;
using Kroiko.Domain.TemplateBuilding;
using Kroiko.Domain.TextFileGeneration;

namespace Kroiko.Domain.ExcelFilesGeneration;

public class FileGeneratorService(IExcelFileGenerator excelFileGenerator, ITextFileGenerator textFileGenerator)
{
    public async Task<List<FileSaveContext>> CreateFiles(ContactInfo contactInfo, IEnumerable<KroikoFile> files, ITemplateBuilder? templateBuilder = null, IFileNameProvider? fileNameProvider = null, string? differentEdgeColor = null, bool? generateTextFiles = null)
    {
        List<FileSaveContext> result = [];
        if (generateTextFiles.HasValue && generateTextFiles.Value)
        {
            result.AddRange(textFileGenerator.CreateTextBasedFile(contactInfo,files));
        }
        var sheets = await templateBuilder.BuildTemplateAsync(contactInfo, files);
        foreach (var sheet in sheets)
        {
            var differentEdgeColorCell = sheet.Cells.FirstOrDefault(c => c.Value == TemplateBuilderBase.DifferentEdgeColorCellFlag);
            if (differentEdgeColorCell is not null)
            {
                differentEdgeColorCell.Value = differentEdgeColor;
            }
        }
        var fileSaveContexts = await excelFileGenerator.GenerateExcelFilesAsync(sheets, fileNameProvider);
        foreach (var file in fileSaveContexts)
        {
            file.FileName = file.FileName.Replace(TemplateBuilderBase.CompanyNameCellFlag, contactInfo.CompanyName);
        }
        result.AddRange(fileSaveContexts);
        return result;
    }
}