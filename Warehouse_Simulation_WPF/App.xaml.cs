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
    #region Fields
    /// <summary>
    /// Main window of the application.
    /// </summary>
    private MainWindow _view = null!;
    /// <summary>
    /// Main view model of the application.
    /// </summary>
    private MainViewModel _viewModel = null!;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        Startup += new StartupEventHandler(App_Startup);
    }

    /// <summary>
    /// Method that starts the application.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Method that saves the log file.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveLogFile(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;
        SaveFileDialog saveFileDialog = new()
        {
            Title = "Save simulation into log file",
            DefaultExt = "json",
            Filter = "Json Files|*.json",
            DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            CheckPathExists = true,
            RestoreDirectory = true,
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            string filepath = saveFileDialog.FileName;
            try
            {
                _viewModel.SaveFile(filepath);
                _view.Back(this, EventArgs.Empty);
            }
            catch (Exception)
            {
                MessageBox.Show("Couldn't save log file!", "Warehouse Simulator", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


    }

    /// <summary>
    /// Method that throws not implemented exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void NewReplay(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Method that loads the config or log file.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadFile(object? sender, (string title, string type) e)
    {
        try
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = e.title,
				DefaultExt = "json",
				Filter = "Json Files|*.json",
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
            };

            if (openFileDialog.ShowDialog() == true)
            {
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

    /// <summary>
    /// Method that handles the closing of the view.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void View_Closing(object? sender, CancelEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to exit?", "Warehouse Simulator", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
        {
            e.Cancel = true;
        }
    }
    #endregion
}
