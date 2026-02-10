using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using Projektmanagement_DesktopApp.ViewModels;
namespace Projektmanagement_DesktopApp.Views;

public partial class WorkersView : UserControl
{
    public WorkersView()
    {
        InitializeComponent();
    }
    
    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var result = await DialogHost.Show(
            this.Resources["DeleteWorkerDialog"],
            "WorkerRootDialog");

        if (result is true && DataContext is WorkersViewModel viewModel)
        {
            await viewModel.DeleteSelectedWorkerAsync();
        }
    }
}
