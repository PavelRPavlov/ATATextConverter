using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ATAFurniture.Server.Services.ExcelGenerator;
using Microsoft.Extensions.Logging;

namespace ATAFurniture.Server.Services;

public class DetailsExtractor(ILogger<DetailsExtractor> logger)
{
    private const char ParameterSplitter = ';';
    
    public List<Detail> ExtractDetailsAsync(MemoryStream stream)
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

    private static List<Detail> ConvertLinesToDetails(string[] lines)
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

        return details;
    }
}