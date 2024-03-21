using System.Windows;
using Warehouse_Simulation_WPF.View;
using Warehouse_Simulation_WPF.ViewModel;
using Warehouse_Simulation_Model.Model;
using System.ComponentModel;
using Microsoft.Win32;
using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly MainWindow _view = null!;
    private readonly MainViewModel _viewModel = null!;
    private Scheduler _model = null!;
    public App()
    {
        Startup += new StartupEventHandler(App_Startup);
    }


    private void App_Startup(object? sender, StartupEventArgs e)
    {
        _viewModel = new MainViewModel(_model);
        _viewModel.NewSimulationStarted += new EventHandler(NewSimulation);

        _view = new MainWindow();
        _view.DataContext = _viewModel;
        _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing);
        _view.Show();
    }



    private async void NewSimulation(object? sender, System.EventArgs e)
    {
        try
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Awari játék betöltése";
            openFileDialog.Filter = "Json Files|*.json";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
  
                _model = new Scheduler(ConfigReader.Read(openFileDialog.FileName));
            }
        }
        catch (Exception)
        {
            MessageBox.Show("Could'nt load file", "Warehouse Simulator", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void View_Closing(object? sender, CancelEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to exit?", "Warehouse Simulator", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
        {
            e.Cancel = true;
        }
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _view.DataContext = _viewModel;
        _view.Show();
    }
}
