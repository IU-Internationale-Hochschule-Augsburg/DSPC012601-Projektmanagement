using Projektmanagement_DesktopApp.Models;

namespace Projektmanagement_DesktopApp.Repositories;

public interface IWorkerRepository
{
    Task<IEnumerable<WorkerModel>> GetAllAsync();
    Task<WorkerModel> AddAsync(WorkerModel model);
    Task UpdateAsync(WorkerModel model);
    Task DeleteAsync(int id);
}