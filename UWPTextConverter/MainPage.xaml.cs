using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPTextConverter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static readonly string XmlTemplateFileName = "template.xml";
        static readonly string XmlDataStartRowFlag = "dataStartRow";
        static readonly string XmlMaterialNameCellFlag = "materialNameCell";
        static readonly string CellsCollectionNodeName = "cells";
        static readonly string NameAttribute = "name";
        static readonly string ValueAttribute = "value";
        static readonly char ParameterSplitter = ';';

        public MainPage()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.InitializeComponent();
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var storageFile = items[0] as StorageFile;
                    await CreateSeparateFiles(storageFile);
                }
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async Task CreateSeparateFiles(StorageFile fi)//, string outputFilesPath)
        {
            var lin = await FileIO.ReadTextAsync(fi);
            var textLines = lin.Split("\r\n");
            var groupedDetals = ReadAndGroupDetailsFromFile(textLines);

            foreach (IGrouping<string, Detail> group in groupedDetals)
            {
                if (group.Key.Contains('?'))
                {
                    continue;
                }

                // Create and fill Excel file
                using (var package = new ExcelPackage())
                {
                    int dataStartRow;
                    ExcelWorksheet defaultSheet = this.CreateWorkSheet(package, group.Key, out dataStartRow);
                    defaultSheet.DefaultColWidth = 30;
                    defaultSheet.Column(3).Width = 15;
                    defaultSheet.Column(4).Width = 70;
                    foreach (Detail detail in group)
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
                        cellD.Value = string.Format("{0} {1}; {2}", detail.LoniraEgdes, detail.Hint, detail.Description);
                        cellD.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        dataStartRow++;
                    }

                    var filePath = DateTime.Now.ToString("yyy-MM-dd") + "_ATAFurniture_" + group.Key;

                    var picker = new FileSavePicker();
                    picker.SuggestedFileName = filePath;
                    picker.FileTypeChoices.Add("Exel", new List<string> { ".xlsx" });
                    picker.CommitButtonText = "Save";
                    picker.SuggestedStartLocation = PickerLocationId.Desktop;

                    StorageFile file = await picker.PickSaveFileAsync();
                    if (file != null)
                    {
                        CachedFileManager.DeferUpdates(file);
                        var a = await package.GetAsByteArrayAsync();
                        await FileIO.WriteBytesAsync(file, a);                        
                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    }
                }
            }
        }

        private IEnumerable<IGrouping<string, Detail>> ReadAndGroupDetailsFromFile(string[] lines)
        {
            List<Detail> details = new List<Detail>();
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string[] separateParameters = line.Split(ParameterSplitter);
                var detail = new Detail(
                    double.Parse(separateParameters[0]),
                    double.Parse(separateParameters[1]),
                    int.Parse(separateParameters[2]),
                    separateParameters[3],
                    int.Parse(separateParameters[4]) == 1,
                    separateParameters[5],
                    int.Parse(separateParameters[6]) == 1,
                    int.Parse(separateParameters[7]) == 1,
                    int.Parse(separateParameters[8]) == 1,
                    int.Parse(separateParameters[9]) == 1,
                    separateParameters[10]
                    );
                details.Add(detail);
            }

            return details.GroupBy((d) => d.Material);
        }

        private ExcelWorksheet CreateWorkSheet(ExcelPackage package, string detailMaterialName, out int dataStartRow)
        {
            ExcelWorksheet defaultSheet;
            dataStartRow = -1;
            var templateDirPath = Path.Combine(Directory.GetCurrentDirectory(), XmlTemplateFileName);
            using (var templateReader = XmlReader.Create(templateDirPath))
            {
                templateReader.Read();

                // Create default sheettemplateReader.ReadToDescendant(XmlRootNodeName))
                var sheetName = templateReader.GetAttribute(NameAttribute);
                package.Workbook.Worksheets.Add(sheetName);
                defaultSheet = package.Workbook.Worksheets[sheetName];

                // Fill basic information and column headers
                if (templateReader.ReadToDescendant(CellsCollectionNodeName))
                {
                    var cellsReader = templateReader.ReadSubtree();
                    while (cellsReader.Read())
                    {
                        var cellName = cellsReader.GetAttribute(NameAttribute);
                        if (!string.IsNullOrEmpty(cellName))
                        {
                            var val = cellsReader.GetAttribute(ValueAttribute);
                            if (cellName == XmlDataStartRowFlag)
                            {
                                dataStartRow = int.Parse(val);
                                break;
                            }
                            if (cellName == XmlMaterialNameCellFlag)
                            {
                                cellName = val;
                                val = detailMaterialName;
                            }
                            var cell = defaultSheet.Cells[cellName];
                            cell.Value = val;

                            // we use fixed width (ExcelWorksheet.DefaultColWidth) because this method throws exception under UWP
                            //cell.AutoFitColumns();
                        }
                    }
                }
            }
            return defaultSheet;
        }
    }
}