using System.Windows;
using Warehouse_Simulation_WPF.View;
using Warehouse_Simulation_WPF.ViewModel;
using System.ComponentModel;
using Microsoft.Win32;

namespace Warehouse_Simulation_WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private MainWindow _view = null!;
    private MainViewModel _viewModel = null!;
    //private Scheduler _model = null!;
    public App()
    {
        Startup += new StartupEventHandler(App_Startup);
    }


    private void App_Startup(object? sender, StartupEventArgs e)
    {
        _viewModel = new MainViewModel();
        _viewModel.NewSimulationStarted += new EventHandler<(string, string)>(LoadFile);
        _viewModel.Replay += new EventHandler<(string, string)>(LoadFile);
        _viewModel.SaveLog += new EventHandler(SaveLogFile);

        _view = new MainWindow();
        _view.DataContext = _viewModel;
        _view.Closing += new CancelEventHandler(View_Closing);
        _view.ReplaySlider.ValueChanged += _viewModel.ReplaySLider_ValueChanged;
        _view.Show();
    }

    private void SaveLogFile(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Title = "Save simulation into log file";
        saveFileDialog.DefaultExt = "json";
        saveFileDialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        saveFileDialog.Filter = "Json Files|*.json";
        if (saveFileDialog.ShowDialog() == true)
        {
            string filepath = saveFileDialog.FileName;
            try
            {
                _viewModel.SaveFile(filepath);
            }
            catch(Exception)
            {
                MessageBox.Show("Couldn't save log file!", "Warehouse Simulator", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }


    }

    private void NewReplay(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void LoadFile(object? sender, (string title, string type) e)
    {
        try
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = e.title;
            openFileDialog.Filter = "Json Files|*.json";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            

            if (openFileDialog.ShowDialog() == true)
            {

                //_model = new Scheduler(ConfigReader.Read(openFileDialog.FileName));
                if (e.type == "config")
                {
                    _viewModel.CreateScheduler(openFileDialog.FileName);
                    _view.MenuGrid.Visibility = Visibility.Collapsed;
                    _view.WindowState = WindowState.Maximized;
                    _view.SimGrid.Visibility = Visibility.Visible;
                    _view.StartGrid.Visibility = Visibility.Visible;
                }
                if (e.type == "log")
                {
                    OpenFileDialog mapDialog = new OpenFileDialog();
                    mapDialog.Title = "Choose map file";
                    mapDialog.Filter = "Map files|*.map";
                    mapDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (mapDialog.ShowDialog() == true)
                    {
                        _viewModel.CreateReplay(openFileDialog.FileName, mapDialog.FileName);
                        _view.MenuGrid.Visibility = Visibility.Collapsed;
                        _view.WindowState = WindowState.Maximized;
                        _view.SimGrid.Visibility = Visibility.Visible;
                        _view.ReplayStartGrid.Visibility = Visibility.Visible;
                    }
                    
                }
                
            }
        }
        catch (Exception)
        {
            MessageBox.Show("Couldn't load file", "Warehouse Simulator", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void View_Closing(object? sender, CancelEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to exit?", "Warehouse Simulator", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
        {
            e.Cancel = true;
        }
    }
}
