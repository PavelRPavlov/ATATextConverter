using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using Microsoft.Win32;

namespace WPFTextConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly string XmlTemplateFileName = "template.xml";
        static readonly string XmlDataStartRowFlag = "dataStartRow";
        static readonly string XmlMaterialNameCellFlag = "materialNameCell";
        static readonly string CellsCollectionNodeName = "cells";
        static readonly string NameAttribute = "name";
        static readonly string ValueAttribute = "value";
        static readonly char ParameterSplitter = ';';

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            string[] droppedFilePath = (string[])e.Data.GetData(DataFormats.FileDrop);
            var fileInfo = new FileInfo(droppedFilePath[0]);
            var groupedDetals = this.ReadAndGroupDetailsFromFile(fileInfo.FullName);
            this.CreateSeparateFiles(groupedDetals, fileInfo.DirectoryName + "\\");
        }

        private IEnumerable<IGrouping<string, Detail>> ReadAndGroupDetailsFromFile(string droppedFilePath)
        {
            // Read input file
            string[] lines = File.ReadAllLines(droppedFilePath);
            List<Detail> details = new List<Detail>();
            foreach (string line in lines)
            {
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

        private void CreateSeparateFiles(IEnumerable<IGrouping<string, Detail>> groupedDetals, string outputFilesPath)
        {
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

                    foreach (Detail detail in group)
                    {
                        defaultSheet.Cells[$"A{dataStartRow}"].Value = detail.Height;
                        defaultSheet.Cells[$"B{dataStartRow}"].Value = detail.Width;
                        defaultSheet.Cells[$"C{dataStartRow}"].Value = detail.Quantity;
                        defaultSheet.Cells[$"D{dataStartRow}"].Value = string.Format("{0} {1}; {2}", detail.LoniraEgdes, detail.Hint, detail.Description);
                        dataStartRow++;
                    }

                    var filePath = outputFilesPath + System.DateTime.Now.ToString("yyy-MM-dd") + "_ATAFurniture_" + group.Key;

                    // Use SaveFileDialog to avoid dealing with user permissions
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "Save Excel sheet";
                    saveFileDialog.Filter = "Excel files|*.xlsx|All files|*.*";
                    saveFileDialog.FileName = filePath;
                    saveFileDialog.InitialDirectory = outputFilesPath;

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        FileInfo fi = new FileInfo(saveFileDialog.FileName);
                        package.SaveAs(fi);
                    }
                }
            }
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
                            cell.AutoFitColumns();
                        }
                    }
                }
            }
            return defaultSheet;
        }
    }
}
