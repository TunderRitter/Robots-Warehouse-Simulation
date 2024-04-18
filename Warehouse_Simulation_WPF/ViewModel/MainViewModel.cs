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
    #region properties
    Scheduler? _scheduler;
    Replay? _replayer;

    private int _row;
    public int Row
    {
        get => _row;
        set
        {
            if (_row != value)
            {
                _row = value;
                OnPropertyChanged();
            }
        }
    }

    private int _col;
    public int Col
    {
        get => _col;
        set
        {
            if (_col != value)
            {
                _col = value;
                OnPropertyChanged();
            }
        }
    }
    public int HeightOfWIndow { get; set; }

    private int _mapHeight;
    public int MapHeight
    {
        get => _mapHeight;
        set
        {
            _mapHeight = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ScrollViewHeight));
        }
    }

    private int _mapWidth;
    public int MapWidth
    {
        get => _mapWidth;
        set
        {
            _mapWidth = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ScrollViewWidth));
        }
    }

    public int ScrollViewHeight => MapHeight + 20;
    public int ScrollViewWidth => MapWidth + 20;

    private int _cellSize;
    public int CellSize
    {
        get => _cellSize;
        set
        {
            _cellSize = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CircleSize));
        }
    }

    public int CircleSize => CellSize - 10;

    private int _zoomValue;
    public int ZoomValue
    {
        get => _zoomValue;
        set
        {
            _zoomValue = value;
            OnPropertyChanged();
        }
    }

    private int _intValue;
    public string IntValue
    {
        get => _intValue.ToString();
        set {
            if (int.TryParse(value, out int val) && val != _intValue)
            {
                _intValue = val;
                OnPropertyChanged();
            }
        }
    }

    private int _stepValue;
    public string StepValue
    {
        get => _stepValue.ToString();
        set {
            if (int.TryParse(value, out int val) && val != _stepValue)
            {
                _stepValue = val;
                OnPropertyChanged();
            }
        }
    }

    private bool _canOrder;
    public bool CanOrder
    {
        get => _canOrder;
        set
        {
            _canOrder = value;
            OnPropertyChanged();
        }
    }

    private int _stepCount;

    public string StepCount
    {
        get { return _stepCount.ToString(); }
        set { _stepCount = int.Parse(value); OnPropertyChanged(nameof(StepCount)); StepMethod(value); }
    }
    private int _robotNumber;

    public string RobotNumber
    {
        get { return _robotNumber.ToString(); }
        set { _robotNumber = int.Parse(value); OnPropertyChanged(nameof(RobotNumber)); }
    }
    private int _targetLeft;

    public string TargetLeft
    {
        get { return _targetLeft.ToString(); }
        set { _targetLeft = int.Parse(value); OnPropertyChanged(nameof(TargetLeft)); }
    }
    private int _maxMap;
    public int MaxMap
    {
        get { return _replayer == null ? 10 : _replayer.Maps.Length; }
        set { _maxMap = (int)(value); OnPropertyChanged(nameof(MaxMap)); }
    }

    LinearGradientBrush South = new LinearGradientBrush(Colors.LightCyan, Colors.DarkCyan, 90.0);
    LinearGradientBrush North = new LinearGradientBrush(Colors.DarkCyan, Colors.LightCyan, 90.0);
    LinearGradientBrush East = new LinearGradientBrush(Colors.LightCyan, Colors.DarkCyan, 0.0);
    LinearGradientBrush West = new LinearGradientBrush(Colors.DarkCyan, Colors.LightCyan, 0.0);

    LinearGradientBrush Target = new LinearGradientBrush(Colors.Salmon, Colors.Salmon, 0.0);
    LinearGradientBrush InactiveTarget = new LinearGradientBrush(Colors.LightGray, Colors.LightGray, 0.0);

    LinearGradientBrush Wall = new LinearGradientBrush(Colors.DarkSlateGray, Colors.DarkSlateGray, 0.0);
    LinearGradientBrush Floor = new LinearGradientBrush(Colors.White, Colors.White, 0.0);



    public event EventHandler? ExitGame;


    public ObservableCollection<CellState> Cells { get; private set; }

    public DelegateCommand NewSimulation { get; private set; }
    public DelegateCommand LoadReplay { get; private set; }

    public DelegateCommand StartSim { get; private set; }
    public DelegateCommand StartReplay { get; private set; }
    public DelegateCommand Exit { get; private set; }

    public DelegateCommand Zoom { get; private set; }
    public DelegateCommand StepCommand { get; init; }
    public DelegateCommand IntCommand { get; init; }
    public DelegateCommand BackToMenu { get; init; }
    public DelegateCommand StepTo { get; init; }

    #endregion

    #region events
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<(string, string)>? NewSimulationStarted;
    public event EventHandler<(string, string)>? Replay;

    #endregion
    public MainViewModel()
    {
        ZoomValue = 1;
        IntValue = "1000";
        StepValue = "100";

        Zoom = new DelegateCommand(ZoomMethod);
        NewSimulation = new DelegateCommand(param => OnNewSimulation());
        StartSim = new DelegateCommand(param => OnSimStart());
        StartReplay = new DelegateCommand(param => OnReplayStart());
        LoadReplay = new DelegateCommand(param => OnReplay());
        Exit = new DelegateCommand(param => OnExitGame());
        StepCommand = new DelegateCommand(value => StepValue = (string?)value ?? StepValue);
        IntCommand = new DelegateCommand(value => IntValue = (string?)value ?? IntValue);
        BackToMenu = new DelegateCommand(OnBackToMenu);
        StepTo = new DelegateCommand(param => StepMethod(param));

        Cells = new ObservableCollection<CellState>();
        
    }

    private void Scheduler_ChangeOccurred(object? sender, EventArgs e)
    {
        if (_scheduler == null) return;
        try
        {
            Application.Current?.Dispatcher?.Invoke(UpdateSimMap);
        }
        catch { }
    }
    private void OnBackToMenu(object? parameter)
    {
        Cells.Clear();
        IntValue = "1000";
        StepValue = "100";
        CanOrder = false;
        ZoomValue = 1;
        _scheduler.runs = false;
        _scheduler = null;
    }

    private void StepMethod(object? parameter)
    {
        if (parameter != null && _replayer != null)
        {
            string? p = parameter.ToString();
            if (p != null)
            {
                switch (p)
                {
                    case "+":
                        _replayer.StepFwd();
                        break;
                    case "-":
                        _replayer.StepBack();
                        break;
                    default:
                        int s = int.Parse(p);
                        _replayer.SkipTo(s);
                        break;
                }
            }
        }
    }
    public void CreateScheduler(string path)
    {
        try
        {
            _scheduler = new Scheduler(ConfigReader.Read(path));
            _scheduler.ChangeOccurred += new EventHandler(Scheduler_ChangeOccurred);
            CalculateHeight(_scheduler.Map);
            Row = _scheduler.Map.GetLength(0);
            Col = _scheduler.Map.GetLength(1);
            CreateSimMap();

        }
        catch (Exception)
        {
            throw;
        }
    }
    public void CreateReplay(string logPath, string mapPath)
    {
        try
        {
            _replayer = new Replay(logPath, mapPath);
            _replayer.ChangeOccurred += new EventHandler<int>(Replayer_ChangeOccured);
            CalculateHeight(_replayer.Map);
            Row = _replayer.Map.GetLength(0);
            Col = _replayer.Map.GetLength(1);
            CreateReplayMap();
            MaxMap = _replayer.Maps.Length;
           
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void Replayer_ChangeOccured(object? sender, int e)
    {
        throw new NotImplementedException();
    }

    private void CalculateHeight(Cell[,] map)
    {
        int height = (int)SystemParameters.PrimaryScreenHeight - 200;
        int width = (int)SystemParameters.PrimaryScreenWidth - 450;
        if (height * ((double)map.GetLength(1) / map.GetLength(0)) > width)
        {
            // width is max
            height = (int)(width * ((double)map.GetLength(0) / map.GetLength(1)));
        }
        else
        {
            // height is max
            width = (int)(height * ((double)map.GetLength(1) / map.GetLength(0)));
        }
        MapHeight = height;
        MapWidth = width;
        CellSize = MapHeight / map.GetLength(0);
    }

    private void CreateMap(Cell[,] map)
    {
        Cells.Clear();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                Cell cell = map[i, j];
                String? id = ((cell is Floor s) ? (s.Robot != null ? s.Robot.Id.ToString() : (s.Target != null ? s.Target.Id.ToString() : String.Empty)) : String.Empty);

                Cells.Add(new CellState
                {
                    X = i,
                    Y = j,
                    Circle = CircleColor(cell),
                    Square = (cell is Floor) ? Floor : Wall,
                    Id = id == null ? String.Empty : id
                });
            }
        }
    }

    private void CreateSimMap()
    {
        if (_scheduler == null) return;
        StepCount = _scheduler.Step.ToString();
        RobotNumber = "0";
        TargetLeft = "0";
        CreateMap(_scheduler.Map);
    }

    private void CreateReplayMap()
    {
        if (_replayer == null) return;
        CreateMap(_replayer.Map);
    }
    private void Cell_TargetPlaced(object? sender, EventArgs c)
    {
        if (_scheduler == null) return;
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

    private void UpdateSimMap()
    {
        if (_scheduler == null) return;
        StepCount = _scheduler.Step.ToString();
        RobotNumber = "0";
        TargetLeft = "0";
        for (int i = 0; i < Cells.Count; i++)
        {
            int idx = i;
            Cell cell = _scheduler.Map[Cells[idx].X, Cells[idx].Y];
            String? id = ((cell is Floor s) ? (s.Robot != null ? s.Robot.Id.ToString() : (s.Target != null ? s.Target.Id.ToString() : String.Empty)) : String.Empty);
            Cells[idx].Circle = CircleColor(cell);
            Cells[idx].Square = (cell is Floor) ? Brushes.White : Brushes.DarkSlateGray;
            Cells[idx].Id = id == null ? String.Empty : id;

        }
    }
    private void UpdateReplayMap(int[,] map)
    {
        if (_replayer == null) return;
        for (int i = 0; i < Cells.Count; i++)
        {
            int idx = i;

        }
    }

    private LinearGradientBrush CircleColor(Cell cell)
    {
        if (cell is Wall)
        {
            return Wall;
        }
        if (cell is Floor floor)
        {
            if (floor.Robot != null)
            {
                if (floor.Robot.Direction == Direction.N) return North;
                if (floor.Robot.Direction == Direction.S) return South;
                if (floor.Robot.Direction == Direction.E) return East;
                if (floor.Robot.Direction == Direction.W) return West;
            }
            if (floor.Target != null)
            {
                if (floor.Target.Active) return Target;
                return InactiveTarget;
            }
            return Floor;
        }
        return Floor;
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
        Task.Run(() => _scheduler.Schedule());
    }

    private void OnNewSimulation()
    {
        NewSimulationStarted?.Invoke(this, ("Choose config file","config"));
    }

    private void OnReplay()
    {
        Replay?.Invoke(this, ("Choose log file","log"));
    }

    private void OnReplayStart()
    {

    }

    private void OnExitGame()
    {
        ExitGame?.Invoke(this, EventArgs.Empty);
    }

    
    

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
