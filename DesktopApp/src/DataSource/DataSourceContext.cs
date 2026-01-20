using System.IO;
using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataClass;
using Task = Projektmanagement_DesktopApp.DataClass.Task;

namespace Projektmanagement_DesktopApp.DataSource;

public class DataSourceContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Ressource> Ressources {get; set;}
    public DbSet<Task> Task {get; set;}
    public DbSet<Worker> Workers {get; set;}
    public DbSet<TaskRevision> TaskRevisions {get; set;}
    
    public string DbPath { get; }
    
    public DataSourceContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        // Path.Combine ist sicherer als manuelle Slashes
        var directoryPath = Path.Combine(path, "DesktopApp", "data");
        
        // Sicherstellen, dass der Ordner existiert
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        DbPath = Path.Combine(directoryPath, "database.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}