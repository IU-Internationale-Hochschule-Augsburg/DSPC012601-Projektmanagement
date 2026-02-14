using System.Collections.ObjectModel;
using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.Models;

namespace Projektmanagement_DesktopApp.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskModel>> GetAllAsync();
    Task<TaskModel> AddAsync(TaskModel task);
    Task<IEnumerable<TaskModel>> GetAllForProjectAsync(Project project);
}
