using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Warehouse_Simulation_WPF.ViewModel;



public class CellState : INotifyPropertyChanged
{
    private int _x;
    public int X
    {
        get { return _x; }
        set
        {
            if (_x != value)
            {
                _x = value;
                OnPropertyChanged(nameof(X));
            }
        }
    }

    private int _y;
    public int Y
    {
        get { return _y; }
        set
        {
            if (_y != value)
            {
                _y = value;
                OnPropertyChanged(nameof(Y));
            }
        }
    }

    private Brush _square;
    public Brush Square
    {
        get { return _square; }
        set
        {
            if (_square != value)
            {
                _square = value;
                OnPropertyChanged(nameof(Square));
            }
        }
    }

    private LinearGradientBrush _circle;
    public LinearGradientBrush Circle
    {
        get { return _circle; }
        set
        {
            if (_circle != value)
            {
                _circle = value;
                OnPropertyChanged(nameof(Circle));
            }
        }
    }

    private string _id;
    public string Id
    {
        get { return _id; }
        set
        {
            if (_id != value)
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
    }

    private int _radius = 0;
    public int Radius
    {
        set
        {
            if (_radius != value)
            {
                _radius = value;
                OnPropertyChanged(nameof(Corners));
            }
        }
    }

    private int[] _corners = [0, 0, 0, 0];
    public string Corners => string.Join(',', _corners.Select(e => e *= _radius));
    public int[] SetCorners
    {
        set
        {
            if (!_corners.SequenceEqual(value))
            {
                _corners = value;
                OnPropertyChanged(nameof(Corners));
            }
        }
    }


    public CellState()
    {
        _square = new LinearGradientBrush();
        _circle = new LinearGradientBrush();
        _id = "";
        CellClick = new DelegateCommand(CellClickMethod);
    }
    private void CellClickMethod(object? parameter)
    {
        CellClicked?.Invoke(this, new CellCoordinates(X, Y));
    }
    public DelegateCommand CellClick { get; set; }
    public event EventHandler? CellClicked;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class CellCoordinates : EventArgs
{
    public int X { get; set; }
    public int Y { get; set; }

    public CellCoordinates(int x, int y)
    {
        X = x; Y = y;
    }
}
