using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.DataSource;
using Projektmanagement_DesktopApp.Models;

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
