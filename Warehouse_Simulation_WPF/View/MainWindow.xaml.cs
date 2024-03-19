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
    private void MenuButton_Click(object sender, EventArgs e)
    {
        this.WindowState = WindowState.Maximized;
    }

}