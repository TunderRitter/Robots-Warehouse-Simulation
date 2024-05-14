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


    private void HideStartGrid(object sender, EventArgs e)
    {
        StartGrid.Visibility = Visibility.Collapsed;
        InfoGrid.Visibility = Visibility.Visible;
        OnlineGrid.Visibility = Visibility.Visible;
        EndButton.Visibility = Visibility.Visible;
    }

    private void HideReplayStartGrid(object sender, EventArgs e)
    {
        ReplayStartGrid.Visibility = Visibility.Collapsed;
        InfoGrid.Visibility = Visibility.Visible;
        ReplayButtons2.Visibility = Visibility.Visible;
        SlowButton.Visibility = Visibility.Visible;
        FastButton.Visibility = Visibility.Visible;
    }

    private void ShowPathButtons(object sender, EventArgs e)
    {
        if (showinc.Visibility == Visibility.Collapsed)
        {
            showinc.Visibility = Visibility.Visible;
            showdec.Visibility = Visibility.Visible;
        }
        else
        {
            showinc.Visibility = Visibility.Collapsed;
            showdec.Visibility = Visibility.Collapsed;
        }
    }

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