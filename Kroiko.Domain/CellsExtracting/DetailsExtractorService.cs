using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace Kroiko.Domain.CellsExtracting;

public interface IDetailsExtractor
{
    Task<List<Detail>> ExtractDetails(MemoryStream stream);
    Task<List<Detail>> ExtractDetails(string fileContent);
}

public class DetailsExtractor(ILogger<DetailsExtractor> logger) : IDetailsExtractor
{
    private const char ParameterSplitter = ';';
    
    public Task<List<Detail>> ExtractDetails(MemoryStream stream)
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
    
    public Task<List<Detail>> ExtractDetails(string fileContent)
    {
        var textLines = fileContent.Split("\r\n");
        return ConvertLinesToDetails(textLines);
    }

    private Task<List<Detail>> ConvertLinesToDetails(string[] lines)
    {
        var details = new List<Detail>();
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var separateParameters = line.Split(ParameterSplitter);
            var detail = separateParameters.Length switch
            {
                11 => ExtractOldDetailFormat(separateParameters),
                23 => ExtractLatestDetailFormat(separateParameters),
                _ => null
            };

            if (detail is null)
            {
                logger.LogError("At least one line did not match the required format");
                return Task.FromResult(new List<Detail>());
            }

            details.Add(detail);
        }

        return Task.FromResult(details);
    }

    private Detail ExtractLatestDetailFormat(ReadOnlySpan<string> separateParameters)
    {
        try
        {
            return new(
                double.Parse(CleanupEmptyString(separateParameters[0]), CultureInfo.InvariantCulture),
                double.Parse(CleanupEmptyString(separateParameters[1]), CultureInfo.InvariantCulture),
                int.Parse(CleanupEmptyString(separateParameters[2])),
                separateParameters[3],
                int.Parse(CleanupEmptyString(separateParameters[4])) == 1,
                int.Parse(CleanupEmptyString(separateParameters[5])) == 1,
                int.Parse(CleanupEmptyString(separateParameters[6])) == 1,
                int.Parse(CleanupEmptyString(separateParameters[7])) == 1,
                int.Parse(CleanupEmptyString(separateParameters[8])) == 1,
                separateParameters[9],
                int.Parse(CleanupEmptyString(separateParameters[10])),
                double.Parse(CleanupEmptyString(separateParameters[11]), CultureInfo.InvariantCulture),
                double.Parse(CleanupEmptyString(separateParameters[12]), CultureInfo.InvariantCulture),
                double.Parse(CleanupEmptyString(separateParameters[13]), CultureInfo.InvariantCulture),
                double.Parse(CleanupEmptyString(separateParameters[14]), CultureInfo.InvariantCulture),
                double.Parse(CleanupEmptyString(separateParameters[15]), CultureInfo.InvariantCulture),
                separateParameters[16],
                separateParameters[17],
                separateParameters[18],
                separateParameters[19],
                separateParameters[20],
            double.Parse(CleanupEmptyString(separateParameters[21]), CultureInfo.InvariantCulture),
            double.Parse(CleanupEmptyString(separateParameters[22]), CultureInfo.InvariantCulture)
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while parsing latest detail format");
            return null;
        }
    }

    private string CleanupEmptyString(string val)
    {
        return string.IsNullOrEmpty(val) ? "0" : val;
    }

    private Detail ExtractOldDetailFormat(ReadOnlySpan<string> separateParameters)
    {
        try
        {
            return new(
                double.Parse(CleanupEmptyString(separateParameters[0]), CultureInfo.InvariantCulture),
                double.Parse(CleanupEmptyString(separateParameters[1]), CultureInfo.InvariantCulture),
                int.Parse(CleanupEmptyString(separateParameters[2])),
                separateParameters[3],
                int.Parse(CleanupEmptyString(separateParameters[4])) == 1,
                int.Parse(CleanupEmptyString(separateParameters[5])) == 1,
                int.Parse(CleanupEmptyString(separateParameters[6])) == 1,
                int.Parse(CleanupEmptyString(separateParameters[7])) == 1,
                int.Parse(CleanupEmptyString(separateParameters[8])) == 1,
                separateParameters[9],
                int.Parse(CleanupEmptyString(separateParameters[10])),
                0, 0, 0, 0, 0, "","", "", "", "",0,0
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while parsing old detail format");
            return null;
        }
    }
}