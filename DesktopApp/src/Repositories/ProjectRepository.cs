using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.DataSource;
using Projektmanagement_DesktopApp.Models;
using Task = System.Threading.Tasks.Task;

namespace Projektmanagement_DesktopApp.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly DataSourceContext _context;

    public ProjectRepository(DataSourceContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<ProjectModel>> GetAllAsync()
    {
        var projects = await _context.Projects.ToListAsync();
        return projects.Select(MapToModel);
    }

    public async Task<ProjectModel> AddAsync(ProjectModel model)
    {
        var entity = new Project
        {
            Name = model.Name,
            Description = model.Description
        };

        _context.Projects.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.id;
        model.CreatedAt = entity.createDate;
        return model;
    }

    public async Task UpdateAsync(ProjectModel project)
    {
        var entity = await _context.Projects.FindAsync(project.Id);
        if (entity != null)
        {
            entity.Name = project.Name;
            entity.Description = project.Description;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await _context.Projects
            .Where(p => p.id == id)
            .ExecuteDeleteAsync();

        if (deleted == 0)
        {
            throw new InvalidOperationException($"Projekt mit ID {id} wurde nicht gefunden.");
        }
    }

    private static ProjectModel MapToModel(Project entity)
    {
        return new ProjectModel
        {
            Id = entity.id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.createDate
        };
    }
}