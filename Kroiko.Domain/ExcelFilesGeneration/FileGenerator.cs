using Kroiko.Domain.ExcelFilesGeneration.XlsxWrapper;
using Kroiko.Domain.TemplateBuilding;
using Kroiko.Domain.TextFileGeneration;

namespace Kroiko.Domain.ExcelFilesGeneration;

public interface IFileGenerator
{
    Task<List<FileSaveContext>> CreateTextFiles(IEnumerable<KroikoFile> files);
    Task<List<FileSaveContext>> CreateExcelFiles(IEnumerable<KroikoFile> files, ITemplateBuilder templateBuilder, IFileNameProvider fileNameProvider);
}
public class FileGenerator(
    IExcelFileGenerator еxcelFileGenerator,
    ITextFileGenerator textFileGenerator) : IFileGenerator
{
    public Task<List<FileSaveContext>> CreateTextFiles(IEnumerable<KroikoFile> files)
        => Task.FromResult(textFileGenerator.CreateTextBasedFile(files));
    public async Task<List<FileSaveContext>> CreateExcelFiles(IEnumerable<KroikoFile> files, ITemplateBuilder templateBuilder, IFileNameProvider fileNameProvider)
    {
        var sheets = await templateBuilder.BuildTemplateAsync(files);
        return await еxcelFileGenerator.GenerateExcelFilesAsync(sheets, fileNameProvider);
    }
}