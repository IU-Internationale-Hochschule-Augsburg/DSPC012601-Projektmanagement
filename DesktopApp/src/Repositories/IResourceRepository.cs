using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.Models;
using Task = System.Threading.Tasks.Task;

namespace Projektmanagement_DesktopApp.Repositories;

public interface IResourceRepository
{
    Task<IEnumerable<ResourceModel>> GetAllAsync();
    Task<ResourceModel> AddAsync(ResourceModel resource);
    Task UpdateAsync(ResourceModel resource);
    Task DeleteAsync(int id);
    Task<IEnumerable<ResourceModel>> GetAllForProjectAsync(Project project);
}