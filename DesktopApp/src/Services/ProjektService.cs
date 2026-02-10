using Projektmanagement_DesktopApp.Repositories;

namespace Projektmanagement_DesktopApp.Services;

public class ProjektService
{
    public void UpdateProjektTimeline(int projectID)
    {
        var dbContext = App.DbContext;

        // Initialize Repositories
        var projectRepo = new ProjectRepository(dbContext);
        
        
        // Load all Task associated with the projekt
        var taskService = new TaskService(new TaskRepository(dbContext));
        
        var tasks = taskService.loadTasksBasedOnPoject(projectID);

        foreach (var task in tasks)
        {
            Console.WriteLine($"Task ID: {task.Id}, Start: {task.StartDate}, End: {task.EndDate}");
        }
        
    }
    
}