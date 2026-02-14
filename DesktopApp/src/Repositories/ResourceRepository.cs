using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.DataSource;
using Projektmanagement_DesktopApp.Models;
using Task = System.Threading.Tasks.Task;

namespace Projektmanagement_DesktopApp.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly DataSourceContext _context;

    public ResourceRepository(DataSourceContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<ResourceModel>> GetAllAsync()
    {
        var resources = await _context.Ressources.ToListAsync();
        return resources.Select(MapToModel);
    }

    public async Task<ResourceModel> AddAsync(ResourceModel model)
    {
        var entity = new Ressource
        {
            Name = model.Name,
            Count = model.Count,
            Project = model.Project != null ? await _context.Projects.FindAsync(model.Project.Id) : null
        };

        _context.Ressources.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.Id;
        model.CreatedAt = entity.CreateDate;
        return model;
    }
    
    public async Task UpdateAsync(ResourceModel model)
    {
        var entity = await _context.Ressources.FindAsync(model.Id);
        if (entity != null)
        {
            entity.Name = model.Name;
            entity.Count = model.Count;
            // vllt. noch was für die Projekt zuordnung hinzufügen?
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task DeleteAsync(int id)
    {
        var deleted = await _context.Ressources
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();

        if (deleted == 0)
        {
            throw new InvalidOperationException($"Resource mit ID {id} wurde nicht gefunden.");
        }
    }

    public async Task<IEnumerable<ResourceModel>> GetAllForProjectAsync(Project project)
    {
        var tasks = await _context.Ressources
            .Where(t => t.Project == project)
            .ToListAsync();
        return tasks.Select(MapToModel);
    }

    private static ResourceModel MapToModel(Ressource entity)
    {
        return new ResourceModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Count = entity.Count,
            Project = entity.Project != null ? new ProjectModel { Id = entity.Project.Id, Name = entity.Project.Name } : null,
            CreatedAt = entity.CreateDate
        };
    }
}
