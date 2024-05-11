using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Warehouse_Simulation_WPF.ViewModel;

/// <summary>
/// Class that represents the state of a cell.
/// </summary>
public class CellState : INotifyPropertyChanged
{
    #region Properties
    private int _x;
    /// <summary>
    /// Property that represents the x coordinate of the cell.
    /// </summary>
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
    /// <summary>
    /// Property that represents the y coordinate of the cell.
    /// </summary>
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
    /// <summary>
    /// Property that represents the square brush of the cell.
    /// </summary>
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
    /// <summary>
    /// Property that represents the circle brush of the cell.
    /// </summary>
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
    /// <summary>
    /// Property that represents the id of the cell.
    /// </summary>
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
    /// <summary>
    /// Property that represents the radius of the brushes.
    /// </summary>
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
    /// <summary>
    /// Property that represents the corners of the brushes.
    /// </summary>
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
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="CellState"/> class.
    /// </summary>
    public CellState()
    {
        _square = new LinearGradientBrush();
        _circle = new LinearGradientBrush();
        _id = "";
        CellClick = new DelegateCommand(CellClickMethod);
    }
    /// <summary>
    /// Method that handles the cell click event.
    /// </summary>
    /// <param name="parameter"></param>
    private void CellClickMethod(object? parameter)
    {
        CellClicked?.Invoke(this, new CellCoordinates(X, Y));
    }
    /// <summary>
    /// Command that handles the cell click event.
    /// </summary>
    public DelegateCommand CellClick { get; set; }
    /// <summary>
    /// Event that handles the cell click event.
    /// </summary>
    public event EventHandler? CellClicked;
    /// <summary>
    /// Event that handles the property changed event.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// Method that handles the property changed event.
    /// </summary>
    /// <param name="propertyName"></param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion
}

/// <summary>
/// Class that represents the cell coordinates.
/// </summary>
public class CellCoordinates : EventArgs
{
    #region Properties
    /// <summary>
    /// Property that represents the x coordinate of the cell.
    /// </summary>
    public int X { get; set; }
    /// <summary>
    /// Property that represents the y coordinate of the cell.
    /// </summary>
    public int Y { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="CellCoordinates"/> class.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public CellCoordinates(int x, int y)
    {
        X = x; Y = y;
    }
    #endregion
}
