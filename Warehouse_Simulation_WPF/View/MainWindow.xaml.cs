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
        MenuGrid.Visibility = Visibility.Collapsed;
        this.WindowState = WindowState.Maximized;
        SimGrid.Visibility = Visibility.Visible;
    }
    private void SimulationButton_Click(object sender, EventArgs e)
    {
        MenuGrid.Visibility = Visibility.Collapsed;
        this.WindowState = WindowState.Maximized;
        SimGrid.Visibility = Visibility.Visible;
    }



}