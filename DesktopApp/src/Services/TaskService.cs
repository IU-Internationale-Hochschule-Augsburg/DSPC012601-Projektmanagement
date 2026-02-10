using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;

namespace Projektmanagement_DesktopApp.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public System.Threading.Tasks.Task updateTaskTime()
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public List<TaskModel> loadTasksBasedOnPoject(int projektID)
    {
        var tasks = _taskRepository.getTasksByProjectId(projektID).Result;
        return tasks.ToList();
    }

    // Recalculate timeline for a project. Throws InvalidOperationException on cycles or other errors.
    public async Task<List<TaskModel>> RecalculateProjectTimelineAsync(int projectId, bool persistChanges = true)
    {
        // Load tasks
        var tasks = (await _taskRepository.GetTasksByProjectIdAsync(projectId)).ToList();

        // Build id -> task map
        var taskById = tasks.ToDictionary(t => t.Id);

        // Build adjacency for topological sort and in-degrees
        var inDegree = new Dictionary<int, int>();
        var adj = new Dictionary<int, List<int>>();

        foreach (var t in tasks)
        {
            inDegree[t.Id] = 0;
            adj[t.Id] = new List<int>();
        }

        foreach (var t in tasks)
        {
            if (t.PreviousTaskId.HasValue)
            {
                var pred = t.PreviousTaskId.Value;
                if (!taskById.ContainsKey(pred))
                    throw new InvalidOperationException($"Vorgänger-Task mit Id {pred} für Task {t.Id} nicht gefunden.");

                adj[pred].Add(t.Id);
                inDegree[t.Id]++;
            }
        }

        // Kahn's algorithm for topological sort
        var queue = new Queue<int>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
        var topo = new List<int>();

        while (queue.Count > 0)
        {
            var n = queue.Dequeue();
            topo.Add(n);
            foreach (var m in adj[n])
            {
                inDegree[m]--;
                if (inDegree[m] == 0) queue.Enqueue(m);
            }
        }

        if (topo.Count != tasks.Count)
        {
            throw new InvalidOperationException("Zyklische Abhängigkeit zwischen Aufgaben erkannt. Timeline kann nicht berechnet werden.");
        }

        // Compute dates: for tasks in topo order set StartDate = predecessor.EndDate or baseline
        DateTime baseline = DateTime.Now.Date; // default baseline is today at 00:00

        foreach (var id in topo)
        {
            var task = taskById[id];
            DateTime newStart;
            if (task.PreviousTaskId.HasValue)
            {
                var pred = taskById[task.PreviousTaskId.Value];
                newStart = pred.EndDate;
            }
            else
            {
                // If task already has a StartDate in the future, keep it as earliest start
                newStart = task.StartDate <= DateTime.MinValue ? baseline : task.StartDate;
                if (newStart < baseline) newStart = baseline;
            }

            DateTime newEnd = newStart.AddHours(task.Duration);

            // Update only if changed
            if (task.StartDate != newStart || task.EndDate != newEnd)
            {
                task.StartDate = newStart;
                task.EndDate = newEnd;
            }
        }

        // Persist changes if requested
        if (persistChanges)
        {
            // Update tasks via repository in the computed order
            foreach (var t in taskById.Values)
            {
                await _taskRepository.UpdateAsync(t);
            }
        }

        return tasks;
    }
}