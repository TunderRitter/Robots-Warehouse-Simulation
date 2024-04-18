using System.Windows;

namespace Warehouse_Simulation_WPF.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void ReplayButton_Click(object sender, EventArgs e)
    {
        //MenuGrid.Visibility = Visibility.Collapsed;
        //this.WindowState = WindowState.Maximized;
        //SimGrid.Visibility = Visibility.Visible;
        ReplayStartGrid.Visibility = Visibility.Visible;
    }
    private void SimulationButton_Click(object sender, EventArgs e)
    {
        //MenuGrid.Visibility = Visibility.Collapsed;
        //this.WindowState = WindowState.Maximized;
        //SimGrid.Visibility = Visibility.Visible;
    }
    private void HideStartGrid(object sender, EventArgs e)
    {
        StartGrid.Visibility = Visibility.Collapsed;
        InfoGrid.Visibility = Visibility.Visible;
    }
    private void HideReplayStartGrid(object sender, EventArgs e)
    {
        ReplayStartGrid.Visibility = Visibility.Collapsed;
        InfoGrid.Visibility = Visibility.Visible;
        ReplayButtons2.Visibility = Visibility.Visible;
    }
    private void CloseSim(object sender, EventArgs e)
    {
        SimGrid.Visibility = Visibility.Collapsed;
        StartGrid.Visibility = Visibility.Visible;
        MenuGrid.Visibility = Visibility.Visible;
        this.WindowState = WindowState.Normal;
    }


}