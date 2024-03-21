using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Warehouse_Simulation_Model.Model;

namespace Warehouse_Simulation_WPF.ViewModel;


public class MainViewModel : INotifyPropertyChanged
{
    Scheduler _scheduler;

    private int _row;
    public int Row
    {
        get { return _row; }
        set
        {
            if (_row != value)
            {
                _row = value;
                OnPropertyChanged(nameof(Row));
            }
        }
    }

    private int _col;
    public int Col
    {
        get { return _col; }
        set
        {
            if (_col != value)
            {
                _col = value;
                OnPropertyChanged(nameof(Col));
            }
        }
    }

    private int _mapHeight;

    public int MapHeight
    {
        get { return _mapHeight; }
        set { _mapHeight = value; OnPropertyChanged(nameof(MapHeight)); }
    }

    private int _cellSize;

    public int CellSize
    {
        get { return _cellSize; }
        set { _cellSize = value; OnPropertyChanged(nameof(CellSize)); }
    }
    private int _circleSize;

    public int CircleSize
    {
        get { return _circleSize; }
        set { _circleSize = value; OnPropertyChanged(nameof(CircleSize)); }
    }



    private int _zoomValue;
    public int ZoomValue
    {
        get { return _zoomValue; }
        set { _zoomValue = value; OnPropertyChanged(nameof(ZoomValue)); }
    }

    public event EventHandler? ExitGame;


    public ObservableCollection<CellState> Cells { get; private set; }

    public DelegateCommand NewSimulation { get; private set; }
    public DelegateCommand LoadReplay { get; private set; }
    public DelegateCommand Exit { get; private set; }

    public DelegateCommand Zoom { get; private set; }


    public MainViewModel(Scheduler scheduler)
    {
        ZoomValue = 1;
        _scheduler = scheduler;

        Zoom = new DelegateCommand(ZoomMethod);
        NewSimulation = new DelegateCommand(param => OnNewSimulation());
        LoadReplay = new DelegateCommand(param => OnReplay());
        Exit = new DelegateCommand(param => OnExitGame());


        Cells = new ObservableCollection<CellState>();
        for (int i = 0; i < _scheduler.Map.GetLength(0); i++)
        {
            for (int j = 0; j < _scheduler.Map.GetLength(1); j++)
            {
                Cell cell = _scheduler.Map[i, j];
                String? id = ((cell is Floor s) ? (s.Robot != null ? s.Robot.Id.ToString() : (s.Target != null ? s.Target.Id.ToString() : String.Empty)) : String.Empty);
                Cells.Add(new CellState
                {
                    X = i,
                    Y = j,
                    Circle = (cell is Floor floor) ? ((floor.Robot != null) ? Brushes.MistyRose : Brushes.White) : Brushes.Black,
                    Square = (cell is Floor) ? Brushes.White : Brushes.Black,
                    Id = id == null ? String.Empty : id
                }) ;
            }
        }
    }
    private void CalculateHeight()
    {
        int height = (int)SystemParameters.PrimaryScreenHeight - 200;
        MapHeight = (height / _scheduler.Map.GetLength(0)) * _scheduler.Map.GetLength(0);
        CellSize = height / _scheduler.Map.GetLength(0);
        CircleSize = CellSize - 10;
    }

    private void ZoomMethod(object? parameter)
    {
        if (parameter != null)
        {
            string? p = parameter.ToString();
            if (p != null)
            {
                switch (p)
                {
                    case "Up":
                        ZoomValue = Math.Min(8, ZoomValue + 1);
                        break;
                    case "Down":
                        ZoomValue = Math.Max(1, ZoomValue - 1);
                        break;
                }
            }
        }
    }

    private void OnNewSimulation()
    {

    }

    private void OnReplay()
    {

    }

    private void OnExitGame()
    {
        ExitGame?.Invoke(this, EventArgs.Empty);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? NewSimulationStarted;
    

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
