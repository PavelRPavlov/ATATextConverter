using OfficeOpenXml;
using System.Windows;

namespace WPFTextConverter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
    }
}
