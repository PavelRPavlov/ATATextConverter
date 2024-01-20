using System.Collections.Generic;
using System.Threading.Tasks;
using ATAFurniture.Server.Services.Template;

namespace ATAFurniture.Server.Services.ExcelGenerator;

public interface IExcelFileGenerator
{
    Task<List<FileSaveContext>> GenerateExcelFilesAsync(IList<ISheet> sheets, IFileNameProvider fileNameProvider);
}