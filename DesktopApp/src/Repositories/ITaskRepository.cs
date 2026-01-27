using Projektmanagement_DesktopApp.Models;

namespace Projektmanagement_DesktopApp.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskModel>> GetAllAsync();
    Task<TaskModel> AddAsync(TaskModel task);
}
