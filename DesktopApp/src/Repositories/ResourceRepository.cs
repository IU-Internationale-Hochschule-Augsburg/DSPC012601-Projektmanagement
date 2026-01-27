using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.DataSource;
using Projektmanagement_DesktopApp.Models;

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
