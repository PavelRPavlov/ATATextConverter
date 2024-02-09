namespace Kroiko.Domain.TemplateBuilding;

public interface IFileNameProvider
{
    string GetFileNameForSheet(ISheet? sheet);
}