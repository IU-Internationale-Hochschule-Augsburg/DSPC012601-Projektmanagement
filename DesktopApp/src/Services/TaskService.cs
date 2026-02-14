using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;
using System.Linq;

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

        foreach (var t in tasks.Where(t => t.PreviousTaskId.HasValue))
        {
            var pred = t.PreviousTaskId.Value;
            if (!taskById.ContainsKey(pred))
                throw new InvalidOperationException($"Vorgänger-Task mit Id {pred} für Task {t.Id} nicht gefunden.");

            adj[pred].Add(t.Id);
            inDegree[t.Id]++;
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

        // Forward calculation: for tasks in topo order set StartDate = predecessor.EndDate or baseline
        DateTime baseline = DateTime.Now.Date; // default baseline is today at 00:00

        foreach (var task in topo.Select(id => taskById[id]))
        {
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

    /// <summary>
    /// Rückwärtskalkulation: Berechnet späteste Start-/Endtermine ausgehend von einer Projekt-Deadline.
    /// Gibt ein Dictionary mit TaskId -> (LatestStart, LatestEnd) zurück, ohne Daten zu verändern.
    /// </summary>
    public async Task<Dictionary<int, (DateTime LatestStart, DateTime LatestEnd)>> BackwardCalculateAsync(
        int projectId, DateTime projectDeadline)
    {
        var tasks = (await _taskRepository.GetTasksByProjectIdAsync(projectId)).ToList();
        var taskById = tasks.ToDictionary(t => t.Id);

        // Build adjacency and reverse adjacency
        var adj = new Dictionary<int, List<int>>();       // predecessor -> successors
        var revAdj = new Dictionary<int, List<int>>();    // successor -> predecessors
        var inDegree = new Dictionary<int, int>();

        foreach (var t in tasks)
        {
            adj[t.Id] = new List<int>();
            revAdj[t.Id] = new List<int>();
            inDegree[t.Id] = 0;
        }

        foreach (var t in tasks.Where(t => t.PreviousTaskId.HasValue))
        {
            var pred = t.PreviousTaskId.Value;
            if (!taskById.ContainsKey(pred))
                throw new InvalidOperationException($"Vorgänger-Task mit Id {pred} für Task {t.Id} nicht gefunden.");

            adj[pred].Add(t.Id);
            revAdj[t.Id].Add(pred);
            inDegree[t.Id]++;
        }

        // Topological sort (Kahn's)
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
            throw new InvalidOperationException("Zyklische Abhängigkeit erkannt.");

        // Backward pass: reverse topological order
        var latestDates = new Dictionary<int, (DateTime LatestStart, DateTime LatestEnd)>();

        for (int i = topo.Count - 1; i >= 0; i--)
        {
            var task = taskById[topo[i]];
            DateTime latestEnd;

            // Tasks without successors: latest end = project deadline
            var successors = adj[task.Id];
            if (successors.Count == 0)
            {
                latestEnd = projectDeadline;
            }
            else
            {
                // Latest end = earliest LatestStart of all successors
                latestEnd = successors.Min(succId => latestDates[succId].LatestStart);
            }

            DateTime latestStart = latestEnd.AddHours(-task.Duration);
            latestDates[task.Id] = (latestStart, latestEnd);
        }

        return latestDates;
    }

    /// <summary>
    /// Erstellt eine Vorschau der Terminänderungen ohne zu speichern.
    /// Gibt eine Liste von TimelineChangeProposal zurück.
    /// </summary>
    public async Task<List<TimelineChangeProposal>> PreviewRecalculationAsync(int projectId)
    {
        // Lade aktuelle Tasks
        var currentTasks = (await _taskRepository.GetTasksByProjectIdAsync(projectId)).ToList();

        // Erstelle Kopien der aktuellen Werte
        var proposals = currentTasks.Select(t => new TimelineChangeProposal
        {
            TaskId = t.Id,
            TaskDisplayName = t.DisplayName,
            OldStartDate = t.StartDate,
            OldEndDate = t.EndDate
        }).ToDictionary(p => p.TaskId);

        // Berechne neue Werte ohne zu persistieren
        var recalculated = await RecalculateProjectTimelineAsync(projectId, persistChanges: false);

        foreach (var task in recalculated)
        {
            if (proposals.TryGetValue(task.Id, out var proposal))
            {
                proposal.NewStartDate = task.StartDate;
                proposal.NewEndDate = task.EndDate;
            }
        }

        return proposals.Values.ToList();
    }
}