using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using Projektmanagement_DesktopApp.ViewModels;

namespace Projektmanagement_DesktopApp.Views;

public partial class ProjectsView : UserControl
{
    public ProjectsView()
    {
        InitializeComponent();
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var result = await DialogHost.Show(
            this.Resources["DeleteDialog"],
            "RootDialog");

        if (result is bool boolResult && boolResult)
        {
            if (DataContext is ProjectsViewModel viewModel)
            {
                await viewModel.DeleteSelectedProjectAsync();
            }
        }
    }
}