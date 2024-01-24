using Kroiko.Domain.TemplateBuilding;

namespace Kroiko.Domain.ExcelFilesGeneration;

public interface IExcelFileGenerator
{
    Task<List<FileSaveContext>> GenerateExcelFilesAsync(IList<ISheet> sheets, IFileNameProvider fileNameProvider);
}