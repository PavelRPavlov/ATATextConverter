namespace Kroiko.Domain.TemplateBuilding;

public interface ITemplateBuilder
{
    public Task<IList<ISheet>> BuildTemplateAsync(IEnumerable<KroikoFile> files);
}