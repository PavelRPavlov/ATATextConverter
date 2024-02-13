using Kroiko.Domain.TemplateBuilding;

namespace Kroiko.Domain.ExcelFilesGeneration;

public class FileGeneratorService(IExcelFileGenerator excelFileGenerator)
{
    public async Task<List<FileSaveContext>> CreateFiles(ContactInfo contactInfo, IEnumerable<KroikoFile> files, ITemplateBuilder templateBuilder, IFileNameProvider fileNameProvider, string? differentEdgeColor = null)
    {
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
        return fileSaveContexts;
    }
}