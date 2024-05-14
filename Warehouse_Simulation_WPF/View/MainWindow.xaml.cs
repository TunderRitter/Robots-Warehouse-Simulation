using System.Windows;

namespace Warehouse_Simulation_WPF.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Creates <see cref="MainWindow"/> object.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Method hiding the starting grid.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HideStartGrid(object sender, EventArgs e)
    {
        StartGrid.Visibility = Visibility.Collapsed;
        InfoGrid.Visibility = Visibility.Visible;
        OnlineGrid.Visibility = Visibility.Visible;
        EndButton.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Method hiding the starting grid during replay.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HideReplayStartGrid(object sender, EventArgs e)
    {
        ReplayStartGrid.Visibility = Visibility.Collapsed;
        InfoGrid.Visibility = Visibility.Visible;
        ReplayButtons2.Visibility = Visibility.Visible;
        SlowButton.Visibility = Visibility.Visible;
        FastButton.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Method to set layout when going back to menu.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Back(object sender, EventArgs e)
    {
        SimGrid.Visibility = Visibility.Collapsed;
        OnlineGrid.Visibility = Visibility.Collapsed;
        ReplayButtons2.Visibility = Visibility.Collapsed;
        InfoGrid.Visibility = Visibility.Collapsed;
        MenuGrid.Visibility = Visibility.Visible;
        WindowState = WindowState.Normal;
        EndButton.Visibility = Visibility.Collapsed;
        SlowButton.Visibility = Visibility.Collapsed;
        FastButton.Visibility = Visibility.Collapsed;
    }
}