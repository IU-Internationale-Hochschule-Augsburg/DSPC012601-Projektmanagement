using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;

namespace Projektmanagement_DesktopApp.Services;

public class ProjektService
{
    public async Task UpdateProjektTimeline(int projectId)
    {
        var dbContext = App.DbContext;

        // Initialize Repositories
        var taskRepo = new TaskRepository(dbContext);

        // Load all tasks associated with the project
        var tasks = (await taskRepo.GetTasksByProjectIdAsync(projectId)).ToList();

        if (tasks == null || tasks.Count == 0)
        {
            // nothing to do
            return;
        }

        // Build id -> task map
        var taskById = tasks.ToDictionary(t => t.Id);

        // visited sets to detect cycles and avoid double processing
        var visited = new HashSet<int>();
        var visiting = new HashSet<int>();

        DateTime baseline = DateTime.Now.Date; // earliest allowed start

        // recursive local function: ensures predecessor calculated first
        async Task RecalculateChainAsync(int id)
        {
            if (visited.Contains(id)) return;
            if (visiting.Contains(id))
                throw new InvalidOperationException("Zyklische Abhängigkeit zwischen Aufgaben erkannt. Timeline kann nicht berechnet werden.");

            if (!taskById.TryGetValue(id, out var task))
                throw new InvalidOperationException($"Task mit Id {id} nicht gefunden.");

            visiting.Add(id);

            DateTime newStart;

            if (task.PreviousTaskId.HasValue)
            {
                var predId = task.PreviousTaskId.Value;
                if (!taskById.TryGetValue(predId, out var predecessorTask))
                    throw new InvalidOperationException($"Vorgänger-Task mit Id {predId} für Task {task.Id} nicht gefunden.");

                // ensure predecessor is calculated first
                await RecalculateChainAsync(predId);
                newStart = predecessorTask.EndDate;
            }
            else
            {
                // no predecessor -> keep existing start if it's reasonable, otherwise use baseline
                newStart = task.StartDate;
                if (newStart < baseline) newStart = baseline;
            }

            DateTime newEnd;

            if (task.Duration > 0)
            {
                // duration given -> compute end from start
                newEnd = newStart.AddHours(task.Duration);
            }
            else
            {
                // no duration -> calculate duration from existing EndDate (EndDate - newStart)
                var existingEnd = task.EndDate;
                if (existingEnd <= newStart)
                {
                    // fallback: set 1 hour duration if existing end is invalid
                    existingEnd = newStart.AddHours(1);
                }

                var computedDuration = (int)Math.Ceiling((existingEnd - newStart).TotalHours);
                if (computedDuration < 0) computedDuration = 0;

                task.Duration = computedDuration;
                newEnd = existingEnd;
            }

            // apply changes
            task.StartDate = newStart;
            task.EndDate = newEnd;

            // persist change
            await taskRepo.UpdateAsync(task);

            visiting.Remove(id);
            visited.Add(id);

            // continue with successor if present
            if (task.NextTaskId.HasValue)
            {
                var nextId = task.NextTaskId.Value;
                if (!taskById.ContainsKey(nextId))
                    throw new InvalidOperationException($"Nachfolger-Task mit Id {nextId} für Task {task.Id} nicht gefunden.");

                await RecalculateChainAsync(nextId);
            }
        }

        // Start recursion from all root tasks (those without predecessor)
        foreach (var root in tasks.Where(t => !t.PreviousTaskId.HasValue))
        {
            await RecalculateChainAsync(root.Id);
        }

        // If some tasks haven't been visited yet, they may be part of a cycle or isolated chain; attempt to process them or report
        if (visited.Count != tasks.Count)
        {
            // try processing remaining tasks (in case there are disconnected chains without explicit root)
            foreach (var t in tasks.Where(t => !visited.Contains(t.Id)))
            {
                await RecalculateChainAsync(t.Id);
            }
        }

        // final sanity check
        if (visited.Count != tasks.Count)
        {
            throw new InvalidOperationException("Nicht alle Aufgaben konnten verarbeitet werden (Zyklus oder fehlende Verknüpfungen). Timeline nicht vollständig aktualisiert.");
        }
    }
}