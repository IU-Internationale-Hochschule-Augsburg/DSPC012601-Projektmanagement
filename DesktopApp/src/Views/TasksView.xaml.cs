using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using Projektmanagement_DesktopApp.ViewModels;

namespace Projektmanagement_DesktopApp.Views;

public partial class TasksView : UserControl
{
    public TasksView()
    {
        InitializeComponent();
    }
    
    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        // 1. Ressource laden
        var dialogContent = this.Resources["DeleteTaskDialog"]; 

        // 2. Sicherheitscheck: Wenn null, dann ist der Pfad oder Key falsch
        if (dialogContent == null)
        {
            MessageBox.Show("Fehler: 'DeleteTaskDialog' nicht in TasksView.xaml gefunden!");
            return;
        }

        // 3. Dialog anzeigen
        var result = await DialogHost.Show(dialogContent, "TaskRootDialog");

        if (result is true && DataContext is TasksViewModel viewModel)
        {
            await viewModel.DeleteSelectedTaskAsync();
        }
    }
}
