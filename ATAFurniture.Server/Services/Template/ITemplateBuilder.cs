using System.Collections.Generic;
using System.Threading.Tasks;
using ATAFurniture.Server.Models;
using ATAFurniture.Server.Services.DetailsExtractor;
using ATAFurniture.Server.Services.ExcelGenerator;

namespace ATAFurniture.Server.Services.Template;

public interface ITemplateBuilder
{
    public Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<Detail> details);
}