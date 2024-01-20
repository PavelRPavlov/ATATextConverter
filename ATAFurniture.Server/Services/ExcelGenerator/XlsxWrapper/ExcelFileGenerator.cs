using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ATAFurniture.Server.Services.Template;
using LargeXlsx;
using Microsoft.Extensions.Logging;

namespace ATAFurniture.Server.Services.ExcelGenerator.XlsxWrapper;

public class ExcelFileGenerator(ILogger<ExcelFileGenerator> logger) : IExcelFileGenerator
{
    public async Task<List<FileSaveContext>> GenerateExcelFilesAsync(IList<ISheet> sheets, IFileNameProvider fileNameProvider)
    {
        var result = new List<FileSaveContext>();
        
        foreach (var sheet in sheets)
        {
            using var str = new MemoryStream();
            using var writer = new XlsxWriter(str);
            var lastFilledRow = 0;
            var sheetColumns = sheet.ColumnWidths.ToColumnStyle();
            writer.BeginWorksheet("Sheet1", columns: sheetColumns);
            var groupedCellsByRow = sheet.Cells.GroupBy(c => c.Row);
            foreach (var row in groupedCellsByRow)
            {
                lastFilledRow++;

                while (lastFilledRow < row.Key)
                {
                    writer.SkipRows(1);
                    lastFilledRow++;
                }
                writer.BeginRow();
                
                foreach (var cell in row)
                {
                    if (cell.ContentAlignment == 0)
                    {
                        writer.Write(cell.Value);
                    }
                    else
                    {
                        writer.Write(
                            double.Parse(cell.Value),
                            XlsxStyle.Default.With(
                                alignment: new XlsxAlignment(XlsxAlignment.Horizontal.Center)));
                    }
                }
            }
            writer.Dispose();
            var fileName = fileNameProvider.GetFileNameForSheet(sheet);
            result.Add(new FileSaveContext(fileName, str.ToArray()));
        }

        return result;
    }
}