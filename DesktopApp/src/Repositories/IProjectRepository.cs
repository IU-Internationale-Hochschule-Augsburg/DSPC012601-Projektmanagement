using Projektmanagement_DesktopApp.Models;

namespace Projektmanagement_DesktopApp.Repositories;

public interface IProjectRepository
{
    Task<IEnumerable<ProjectModel>> GetAllAsync();
    Task<ProjectModel> AddAsync(ProjectModel project);
    Task UpdateAsync(ProjectModel project);
    Task DeleteAsync(int id);
}