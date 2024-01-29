using System.Threading.Tasks;
using Kroiko.Domain;
using Kroiko.Domain.CellsExtracting;

namespace ATAFurniture.Server.DataAccess;

public interface IKroikoDataRepository
{
    Task<User> GetUserAsync(string id);
    Task<User> CreateUser(string userId, int credits);
    Task<User> UpdateUser(User dbUser);
    Task<User> RemoveCredits(User dbUser, int i);
    Task<User> UpdateSelectedCompany(User dbUser, SupportedCompany? targetCompany);
}