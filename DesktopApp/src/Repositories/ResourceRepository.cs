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
            name = model.Name,
            count = model.Count,
            projectUid = model.ProjectId
        };

        _context.Ressources.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.id;
        model.CreatedAt = entity.createDate;
        return model;
    }
    
    public async Task UpdateAsync(ResourceModel model)
    {
        var entity = await _context.Ressources.FindAsync(model.Id);
        if (entity != null)
        {
            entity.name = model.Name;
            entity.count = model.Count;
            // vllt. noch was für die Projekt zuordnung hinzufügen?
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task DeleteAsync(int id)
    {
        var deleted = await _context.Ressources
            .Where(p => p.id == id)
            .ExecuteDeleteAsync();

        if (deleted == 0)
        {
            throw new InvalidOperationException($"Resource mit ID {id} wurde nicht gefunden.");
        }
    }

    private static ResourceModel MapToModel(Ressource entity)
    {
        return new ResourceModel
        {
            Id = entity.id,
            Name = entity.name,
            Count = entity.count,
            ProjectId = entity.projectUid,
            CreatedAt = entity.createDate
        };
    }
}
