using Kroiko.Domain.CellsExtracting;

namespace Kroiko.Domain.TemplateBuilding;

public interface ITemplateBuilder
{
    public Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<KroikoFile> files);
}