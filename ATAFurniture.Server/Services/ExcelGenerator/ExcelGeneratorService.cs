using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace ATAFurniture.Server.Services.ExcelGenerator;

public record FileSaveContext(string FileName, byte[] Content);

public record ContactInfo(string CompanyName, string MobileNumber);

public class ExcelGeneratorService(ILogger<ExcelGeneratorService> logger)
{
    private const string XmlTemplateFilePath = "Services/ExcelGenerator/template.xml";
    private const string XmlMaterialNameCellFlag = "{MaterialName}";
    private const string XmlCompanyNameCellFlag = "{CompanyName}";
    private const string XmlMobileNumberCellFlag = "{MobileNumber}";
    private const string XmlTableStartCellFlag = "{TableStart}";
    private const string CellsCollectionNodeName = "cells";
    private const string NameAttribute = "name";
    private const string ValueAttribute = "value";
    private const char ParameterSplitter = ';';

    private ContactInfo _contactInfo;

    public async Task<List<FileSaveContext>> CreateFiles(string rawContent, ContactInfo contactInfo)
    {
        _contactInfo = contactInfo;
        
        string[] textLines;
        try
        {
            textLines = rawContent.Split("\r\n");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while encoding Polybard file");
            throw;
        }
        
        return await ProcessTextInput(textLines);
    }
    
    public async Task<List<FileSaveContext>> CreateFiles(MemoryStream memoryStream, ContactInfo contactInfo)
    {
        _contactInfo = contactInfo;

        string[] textLines;
        try
        {
            var lin = Encoding.UTF8.GetString(memoryStream.ToArray());
            textLines = lin.Split("\r\n");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while encoding Polybard file");
            throw;
        }

        return await ProcessTextInput(textLines);
    }

    private async Task<List<FileSaveContext>> ProcessTextInput(string[] textLines)
    {
        IEnumerable<IGrouping<string, Detail>> groupedMaterials;
        try
        {
            groupedMaterials = ReadAndGroupDetailsFromFile(textLines);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while parsing Polybard file");
            throw;
        }

        var filesToSave = new List<FileSaveContext>();
        foreach (IGrouping<string, Detail> materialGroup in groupedMaterials)
        {
            if (materialGroup.Key.Contains('?'))
            {
                continue;
            }

            filesToSave.Add(await CreateSingleExcelFile(materialGroup));
        }

        return filesToSave;
    }

    private async Task<FileSaveContext> CreateSingleExcelFile(IGrouping<string, Detail> materialGroup)
    {
        using var package = new ExcelPackage();
        int dataStartRow;
        ExcelWorksheet defaultSheet = CreateWorkSheet(package, materialGroup.Key, out dataStartRow);
        defaultSheet.DefaultColWidth = 30;
        defaultSheet.Column(3).Width = 15;
        defaultSheet.Column(4).Width = 70;
        foreach (Detail detail in materialGroup)
        {
            var cellA = defaultSheet.Cells[$"A{dataStartRow}"];
            cellA.Value = detail.Height;
            cellA.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            var cellB = defaultSheet.Cells[$"B{dataStartRow}"];
            cellB.Value = detail.Width;
            cellB.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            var cellC = defaultSheet.Cells[$"C{dataStartRow}"];
            cellC.Value = detail.Quantity;
            cellC.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            var cellD = defaultSheet.Cells[$"D{dataStartRow}"];
            cellD.Value = string.Format("{0}; {1} {2}", detail.LoniraEgdes, detail.Cabinet, detail.CuttingNumber);
            cellD.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            dataStartRow++;
        }

        return new FileSaveContext(
            DateTime.Now.ToString("yyy-MM-dd") +
            "_" +
            _contactInfo.CompanyName +
            "_" +
            materialGroup.Key +
            ".xlsx",
            await package.GetAsByteArrayAsync());
    }
    
    private ExcelWorksheet CreateWorkSheet(ExcelPackage package, string detailMaterialName, out int dataStartRow)
    {
        ExcelWorksheet defaultSheet;
        dataStartRow = -1;
        
        var templateDirPath = Path.Combine(Directory.GetCurrentDirectory(), XmlTemplateFilePath);
        using var templateReader = XmlReader.Create(templateDirPath);
        templateReader.Read();

        // Create default sheettemplateReader.ReadToDescendant(XmlRootNodeName))
        var sheetName = templateReader.GetAttribute(NameAttribute);
        package.Workbook.Worksheets.Add(sheetName);
        defaultSheet = package.Workbook.Worksheets[sheetName];

        // Fill basic information and column headers
        if (!templateReader.ReadToDescendant(CellsCollectionNodeName))
            return defaultSheet;
        
        var cellsReader = templateReader.ReadSubtree();
        while (cellsReader.Read())
        {
            var cellName = cellsReader.GetAttribute(NameAttribute).AsSpan();
            if (cellName.IsEmpty)
            {
                continue;
            }

            var val = cellsReader.GetAttribute(ValueAttribute);
            switch (val)
            {
                case XmlCompanyNameCellFlag :
                    val = _contactInfo.CompanyName;
                    logger.LogInformation("Replacing {{CompanyName}} with {CompanyName}", val);
                    SetCellContent(defaultSheet, cellName, val);
                    break;
                case XmlMobileNumberCellFlag :
                    val = _contactInfo.MobileNumber;
                    logger.LogInformation("Replacing {{MobileNumber}} with {MobileNumber}", val);
                    SetCellContent(defaultSheet, cellName, val);
                    break;
                case XmlMaterialNameCellFlag :
                    val = detailMaterialName;
                    logger.LogInformation("Replacing {{MaterialName}} with {MaterialName}", val);
                    SetCellContent(defaultSheet, cellName, val);
                    break;
                case XmlTableStartCellFlag :
                    dataStartRow = int.Parse(ExtractRowNumberFromCellName(cellName));
                    logger.LogInformation("Extracted {{TableStart}} to be {DataStartRow}", dataStartRow);
                    break;
                default:
                    SetCellContent(defaultSheet, cellName, val);
                    break;
            }
        }

        return defaultSheet;
    }

    private string ExtractRowNumberFromCellName(ReadOnlySpan<char> cellName)
    {
        return cellName[1..].ToString();
    }

    private static void SetCellContent(ExcelWorksheet defaultSheet, ReadOnlySpan<char> cellName, ReadOnlySpan<char> val)
    {
        var cell = defaultSheet.Cells[cellName.ToString()];
        cell.Value = val.ToString();
    }

    private static IEnumerable<IGrouping<string, Detail>> ReadAndGroupDetailsFromFile(string[] lines)
    {
        var details = new List<Detail>();
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var separateParameters = line.Split(ParameterSplitter);
            var detail = new Detail(
                double.Parse(separateParameters[0], CultureInfo.InvariantCulture),
                double.Parse(separateParameters[1], CultureInfo.InvariantCulture),
                int.Parse(separateParameters[2]),
                separateParameters[3],
                int.Parse(separateParameters[4]) == 1,
                int.Parse(separateParameters[5]) == 1,
                int.Parse(separateParameters[6]) == 1,
                int.Parse(separateParameters[7]) == 1,
                int.Parse(separateParameters[8]) == 1,
                separateParameters[9],
                int.Parse(separateParameters[10])
            );
            details.Add(detail);
        }

        return details.GroupBy((d) => d.Material);
    }
}