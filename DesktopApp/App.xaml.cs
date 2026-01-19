using System.Configuration;
using System.Data;
using System.Windows;
using Projektmanagement_DesktopApp.DataSource;

namespace Projektmanagement_DesktopApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static DataSourceContext DbContext { get; private set; } = null!;
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DbContext = new DataSourceContext();

        // EF Core prüft: Existiert die DB? Wenn nein, erstelle sie + Tabellen.
        DbContext.Database.EnsureCreated();
    }
}