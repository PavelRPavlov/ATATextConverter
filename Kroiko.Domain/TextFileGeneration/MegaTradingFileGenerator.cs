using Kroiko.Domain.ExcelFilesGeneration;
using Kroiko.Domain.TemplateBuilding;
using System.Text;

namespace Kroiko.Domain.TextFileGeneration;

public interface ITextFileGenerator {
    List<FileSaveContext> CreateTextBasedFile(ContactInfo contactInfo, IEnumerable<KroikoFile> files);
}
public class MegaTradingFileGenerator : ITextFileGenerator {

    // this is a special separator symbol required by the integration destination
    private const string S = "\u256a";
    public List<FileSaveContext> CreateTextBasedFile(ContactInfo contactInfo, IEnumerable<KroikoFile> files)
    {
        var materials = files.SelectMany(f => f.Details.Cast<MegaTrading>())
            .GroupBy(d => d.Material).ToList();
        var builder = new StringBuilder();
        
        CreateFirstRow(builder);
        
        // there should always be exactly 6 rows, containing different materials
        for (var i = 0; i <= 5; i++)
        {
            if (i >= materials.Count)
            {
                CreateMaterialRow(builder);
            }
            else
            {
                CreateMaterialRow(builder, materials[i].FirstOrDefault());
            }
        }
        
        CreateColumnSizeRow(builder);
        
        foreach (var kroikoFile in files)
        {
            foreach (MegaTrading detail in kroikoFile.Details)
            {
                CreateDetailRow(builder, detail);
            }
        }
        var file = new FileSaveContext($"{contactInfo.CompanyName}-{DateTime.Now.ToShortDateString()}.cut_mt", Encoding.UTF8.GetBytes(builder.ToString()));
        return [file];
    }
    private static void CreateDetailRow(StringBuilder builder, MegaTrading d)
    {
        var rotated = d.Rotated ? "Yes" : "No";
        builder.AppendLine(
        $"{d.Material}{S}{d.Height}{S}{d.Width}{S}{d.Quantity}{S}{rotated}{S}{d.LeftEdge}{S}{d.BottomEdge}{S}{d.RightEdge}{S}{d.TopEdge}{S}{d.EdgeBandingMaterial}{S}{d.Note}{S}");
    }
    private static void CreateColumnSizeRow(StringBuilder builder)
    {
        // the integration destination uses a GridView to visualize all details
        // this is the row defining the size of each column
        builder.AppendLine(
        $"50{S}220{S}80{S}80{S}50{S}50{S}90{S}90{S}90{S}90{S}200{S}200{S}");
    }
    private static void CreateMaterialRow(StringBuilder builder, MegaTrading? detail = null)
    {
        // there should always be exactly 6 rows, containing different materials
        var material = detail == null ? string.Empty : detail.Material;
        var thickness = detail == null ? 18.0 : detail.Thickness;
        builder.AppendLine($"{material}{S}{thickness}{S}True{S}True{S}2800{S}2070{S}");
    }
    private static void CreateFirstRow(StringBuilder builder)
    {
        // holds a boolean value indicating if the material of the edge banding is different from material itself
        // we always use 'False' to avoid inspecting all details multiple times
        builder.AppendLine($"{S}{S}False");
    }
}