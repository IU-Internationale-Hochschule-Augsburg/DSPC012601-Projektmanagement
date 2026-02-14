using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.Models;
using Task = System.Threading.Tasks.Task;

namespace Projektmanagement_DesktopApp.Repositories;

public interface IProjectRepository
{
    Task<IEnumerable<ProjectModel>> GetAllAsync();
    Task<ProjectModel> AddAsync(ProjectModel project);
    Task UpdateAsync(ProjectModel project);
    Task DeleteAsync(int id);
    Task<Project> GetByIdAsync(int id);
}