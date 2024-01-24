using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Kroiko.Domain.CellsExtracting;

public interface IDetailsExtractorService
{
    List<Detail> ExtractDetails(MemoryStream stream);
}

public class DetailsExtractorService(ILogger<DetailsExtractorService> logger) : IDetailsExtractorService
{
    private const char ParameterSplitter = ';';
    
    public List<Detail> ExtractDetails(MemoryStream stream)
    {
        string[] textLines;
        try
        {
            var lin = Encoding.UTF8.GetString(stream.ToArray());
            textLines = lin.Split("\r\n");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while encoding Polybard file");
            throw;
        }

        return ConvertLinesToDetails(textLines);
    }

    private List<Detail> ConvertLinesToDetails(string[] lines)
    {
        var details = new List<Detail>();
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var separateParameters = line.Split(ParameterSplitter);
            if (separateParameters.Length != 16)
            {
                logger.LogError("Invalid number of parameters in line {Line}", line);
                return null;
            }
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
                int.Parse(separateParameters[10]),
                double.Parse(separateParameters[11], CultureInfo.InvariantCulture),
                double.Parse(separateParameters[12], CultureInfo.InvariantCulture),
                double.Parse(separateParameters[13], CultureInfo.InvariantCulture),
                double.Parse(separateParameters[14], CultureInfo.InvariantCulture),
                double.Parse(separateParameters[15], CultureInfo.InvariantCulture)
            );
            details.Add(detail);
        }

        return details;
    }
}