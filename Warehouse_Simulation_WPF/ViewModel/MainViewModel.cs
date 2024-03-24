using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Warehouse_Simulation_Model.Model;
using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_WPF.ViewModel;


public class MainViewModel : INotifyPropertyChanged
{
    Scheduler? _scheduler;

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

    public int HeightOfWIndow { get; set; }

    private int _mapHeight;
    public int MapHeight
    {
        get { return _mapHeight; }
        set { _mapHeight = value; OnPropertyChanged(nameof(MapHeight)); }
    }
    private int _mapWidth;

    public int MapWidth
    {
        get { return _mapWidth; }
        set { _mapWidth = value; OnPropertyChanged(nameof(MapWidth)); }
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

    private int _intValue;

    public string IntValue
    {
        get { return _intValue.ToString(); }
        set {
            if (int.TryParse(value, out int val) && val != _intValue)
            {
                _intValue = val;
                Debug.WriteLine(val);
                OnPropertyChanged();
            }
        }
    }
    private int _stepValue;

    public string StepValue
    {
        get { return _stepValue.ToString(); }
        set {
            if (int.TryParse(value, out int val) && val != _stepValue)
            {
                _stepValue = val;
                OnPropertyChanged();
            }
        }
    }
    private bool canOrder;

    public bool CanOrder
    {
        get { return canOrder; }
        set { canOrder = value; OnPropertyChanged(nameof(CanOrder)); }
    }




    public event EventHandler? ExitGame;


    public ObservableCollection<CellState> Cells { get; private set; }

    public DelegateCommand NewSimulation { get; private set; }
    public DelegateCommand LoadReplay { get; private set; }

    public DelegateCommand StartSim { get; private set; }
    public DelegateCommand Exit { get; private set; }

    public DelegateCommand Zoom { get; private set; }
    public DelegateCommand StepCommand { get; init; }
    public DelegateCommand IntCommand { get; init; }



    public MainViewModel()
    {
        ZoomValue = 1;
        IntValue = "1000";
        StepValue = "100";

        Zoom = new DelegateCommand(ZoomMethod);
        NewSimulation = new DelegateCommand(param => OnNewSimulation());
        StartSim = new DelegateCommand(param => OnSimStart());
        LoadReplay = new DelegateCommand(param => OnReplay());
        Exit = new DelegateCommand(param => OnExitGame());
        StepCommand = new DelegateCommand(value => StepValue = (string?)value ?? StepValue);
        IntCommand = new DelegateCommand(value => IntValue = (string?)value ?? IntValue);

        Cells = new ObservableCollection<CellState>();
        
    }

    private void _scheduler_ChangeOccurred(object? sender, EventArgs e)
    {
        if (_scheduler != null)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateMap());
        }
    }
    public void CreateScheduler(string path)
    {
        try
        {
            _scheduler = new Scheduler(ConfigReader.Read(path));
            _scheduler.ChangeOccurred += new EventHandler(_scheduler_ChangeOccurred);
            CalculateHeight();
            Row = _scheduler.Map.GetLength(0);
            Col = _scheduler.Map.GetLength(1);
            //_scheduler_ChangeOccurred(null, EventArgs.Empty);
            //Debug.WriteLine("scheduler kész");
            CreateMap();

        }
        catch (Exception)
        {
            throw;
        }
    }

    private void CalculateHeight()
    {
        if (_scheduler == null) return;
        int height = (int)System.Windows.SystemParameters.PrimaryScreenHeight - 200;
        MapHeight = (height / _scheduler.Map.GetLength(0)) * _scheduler.Map.GetLength(0) + 15;
        CellSize = MapHeight / _scheduler.Map.GetLength(0);
        CircleSize = CellSize - 10;
        MapWidth = CellSize * _scheduler.Map.GetLength(1) + 15;

        Debug.WriteLine("Mapheight:" + MapHeight);
        Debug.WriteLine("Cellsize: " + CellSize);
        Debug.WriteLine("Circle: " + CircleSize);
        Debug.WriteLine("MapWidth" + MapWidth);
    }
    private void CreateMap()
    {
        if (_scheduler == null) return;
        Cells.Clear();
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
                    //Circle = (cell is Floor floor) ? ((floor.Robot != null) ? Brushes.MistyRose : Brushes.White) : Brushes.Black,
                    Circle = (cell is Floor floor) ? ((floor.Robot != null) ? Brushes.MediumAquamarine : ((floor.Target != null) ? Brushes.Khaki : Brushes.White)) : Brushes.DarkSlateGray,
                    Square = (cell is Floor) ? Brushes.White : Brushes.DarkSlateGray,
                    Id = id == null ? String.Empty : id
                });
                Cells[^1].TargetPlaced += new EventHandler(_cell_TargetPlaced);

            }
        }
    }
    private void _cell_TargetPlaced(object? sender, EventArgs c)
    {
        if (_scheduler == null) { return; }
        if (CanOrder)
        {
            if(c is CellCoordinates coordinates)
            {
                int i = coordinates.X;
                int j = coordinates.Y;
                _scheduler.AddTarget(i, j);
                
            }
        }

    }
    private void UpdateMap()
    {
        if (_scheduler == null) return;
        //Application.Current.Dispatcher.Invoke(() => );
        for (int i = 0; i<Cells.Count; i++)
        {
            int idx = i;
            Cell cell = _scheduler.Map[Cells[idx].X, Cells[idx].Y];
            String? id = ((cell is Floor s) ? (s.Robot != null ? s.Robot.Id.ToString() : (s.Target != null ? s.Target.Id.ToString() : String.Empty)) : String.Empty);
            Cells[idx].Circle = (cell is Floor floor) ? ((floor.Robot != null) ? Brushes.MediumAquamarine : ((floor.Target != null) ? Brushes.Khaki : Brushes.White)) : Brushes.DarkSlateGray;
            Cells[idx].Square = (cell is Floor) ? Brushes.White : Brushes.DarkSlateGray;
            Cells[idx].Id = id == null ? String.Empty : id;

        }
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

    private void OnSimStart()
    {
        if (_scheduler == null) return;
        _scheduler.MaxSteps = _stepValue;
        _scheduler.TimeLimit = _intValue;
        //_scheduler.Schedule();
        Task.Run(() => _scheduler.Schedule());
    }

    private void OnNewSimulation()
    {
        NewSimulationStarted?.Invoke(this, EventArgs.Empty);
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
