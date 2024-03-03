using System.Windows;
using Warehouse_Simulation_WPF.View;
using Warehouse_Simulation_WPF.ViewModel;

namespace Warehouse_Simulation_WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly MainWindow _window = new();
    private readonly MainViewModel _viewModel = new();


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _window.DataContext = _viewModel;
        _window.Show();
    }
}
