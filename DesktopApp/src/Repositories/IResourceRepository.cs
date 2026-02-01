using Projektmanagement_DesktopApp.Models;

namespace Projektmanagement_DesktopApp.Repositories;

public interface IResourceRepository
{
    Task<IEnumerable<ResourceModel>> GetAllAsync();
    Task<ResourceModel> AddAsync(ResourceModel resource);
}
