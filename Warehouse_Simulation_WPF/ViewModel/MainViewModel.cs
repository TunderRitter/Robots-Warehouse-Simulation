using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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
            if (value != _canOrder)
            {
                _canOrder = value;
                if (_showPath && value)
                {
                    ShowPath = false;
                    _pathIdx = -1;
                }
                OnPropertyChanged();
            }
        }
    }
    private bool _showPath;
    public bool ShowPath
    {
        get => _showPath;
        set
        {
            if (value != _showPath)
            {
                if (_canOrder && value)
                {
                    CanOrder = false;
                }
                if (!value)
                {
                    _pathIdx = -1;
                }
                _showPath = value;
                OnPropertyChanged();
            }
        }
    }

    private int _stepCount;
    public int StepCount
    {
        get => _stepCount;
        set
        {
            if (value != _stepCount)
            {
                _stepCount = value;
                OnPropertyChanged();
            }
        }
    }

    private int _robotNumber;
    public string RobotNumber
    {
        get => _robotNumber.ToString();
        set
        {
            if (int.TryParse(value, out int val) && val != _robotNumber)
            _robotNumber = val; 
            OnPropertyChanged();
        }
    }

    private int _targetLeft;
    public string TargetLeft
    {
        get => _targetLeft.ToString();
        set
        {
            if (int.TryParse(value, out int val) && val != _targetLeft)
            {
                _targetLeft = val;
                OnPropertyChanged();
            }
        }
    }

    public int MaxMap => _replayer?.MaxStep ?? 0;

    private string _pauseText;
    public string PauseText
    {
        get { return _pauseText; }
        set { _pauseText = value; OnPropertyChanged(nameof(PauseText)); }
    }
    private string _endText;
    public string EndText
    {
        get { return _endText; }
        set { _endText = value; OnPropertyChanged(nameof(EndText)); }
    }
    private int _pathIdx;


    LinearGradientBrush South = new LinearGradientBrush(Colors.LightCyan, Colors.DarkCyan, 90.0);
    LinearGradientBrush North = new LinearGradientBrush(Colors.DarkCyan, Colors.LightCyan, 90.0);
    LinearGradientBrush East = new LinearGradientBrush(Colors.LightCyan, Colors.DarkCyan, 0.0);
    LinearGradientBrush West = new LinearGradientBrush(Colors.DarkCyan, Colors.LightCyan, 0.0);

    LinearGradientBrush Target = new LinearGradientBrush(Colors.Salmon, Colors.Salmon, 0.0);
    LinearGradientBrush InactiveTarget = new LinearGradientBrush(Colors.LightGray, Colors.LightGray, 0.0);

    LinearGradientBrush Wall = new LinearGradientBrush(Colors.DarkSlateGray, Colors.DarkSlateGray, 0.0);
    LinearGradientBrush Floor = new LinearGradientBrush(Colors.White, Colors.White, 0.0);
    LinearGradientBrush InPath = new LinearGradientBrush(Colors.PaleGreen, Colors.PaleGreen, 0.0);


    public event EventHandler? ExitApp;


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
    public DelegateCommand StepFwd { get; init; }
    public DelegateCommand StepBack { get; init; }
    public DelegateCommand PlayPause { get; init; }
    public DelegateCommand EndCommand { get; init; }
    public DelegateCommand Slow { get; init; }
    public DelegateCommand Fast { get; init; }

    #endregion

    #region events
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<(string, string)>? NewSimulationStarted;
    public event EventHandler<(string, string)>? Replay;
    public event EventHandler? SaveLog;
    #endregion

    public MainViewModel()
    {
        ZoomValue = 1;
        IntValue = "1000";
        StepValue = "100";
        _pathIdx = -1;

        Zoom = new DelegateCommand(ZoomMethod);
        NewSimulation = new DelegateCommand(param => OnNewSimulation());
        StartSim = new DelegateCommand(param => OnSimStart());
        StartReplay = new DelegateCommand(param => OnReplayStart());
        LoadReplay = new DelegateCommand(param => OnReplay());
        Exit = new DelegateCommand(param => OnExitApp());
        StepCommand = new DelegateCommand(value => StepValue = (string?)value ?? StepValue);
        IntCommand = new DelegateCommand(value => IntValue = (string?)value ?? IntValue);
        BackToMenu = new DelegateCommand(OnBackToMenu);
        StepFwd = new DelegateCommand(param => _replayer?.StepFwd());
        StepBack = new DelegateCommand(param => _replayer?.StepBack());
        PlayPause = new DelegateCommand(param => PlayPauseMethod());
        EndCommand = new DelegateCommand(param => EndSimulation());
        Slow = new DelegateCommand(param => SlowReplay());
        Fast = new DelegateCommand(param => FastReplay());

        Cells = new ObservableCollection<CellState>();
        _pauseText = "";
        _endText = "";
    }

    private void FastReplay()
    {
        if (_replayer == null) return;
        _replayer.FasterSpeed();
    }

    private void SlowReplay()
    {
        if (_replayer == null) return;
        _replayer.SlowerSpeed();
    }

    private void EndSimulation()
    {
        if (_scheduler == null) return;
        if (_scheduler.Running)
        {
            _scheduler.Running = false;
            EndText = "SAVE SIMULATION";
        }
        else if (!_scheduler.Running)
        {
            SaveLog?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PlayPauseMethod()
    {
        if (_replayer == null) return;
        if (_replayer.Paused)
        {
            _replayer.Play();
            PauseText = "\u23F8";
            return;
        }
        if (!_replayer.Paused)
        {
            _replayer.Pause();
            PauseText = "\u25B6";
        }
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
        ShowPath = false;
        ZoomValue = 1;
        if (_scheduler != null)
        {
            _scheduler.Running = false;
            _scheduler = null;
        }
        if (_replayer != null)
        {
            _replayer.Pause();
            _replayer = null;
        }
        
    }

    private void StepMethod(int parameter)
    {
        if (_replayer == null) return;
        _replayer.SkipTo(parameter);
    }

    public void CreateScheduler(string path)
    {
        try
        {
            _scheduler = new Scheduler(ConfigReader.Read(path));
            _scheduler.ChangeOccurred += new EventHandler(Scheduler_ChangeOccurred);
            _scheduler.SimFinished += new EventHandler(Scheduler_SimFinished);
            CalculateHeight(_scheduler.Map);
            Row = _scheduler.Map.GetLength(0);
            Col = _scheduler.Map.GetLength(1);
            EndText = "END SIMULATION";
            CreateSimMap();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public void SaveFile(string path)
    {
        if (_scheduler == null) return;
        try
        {
            _scheduler.WriteToFile(path);
            OnBackToMenu(null);
        }
        catch (Exception) { throw; }
    }

    private void Scheduler_SimFinished(object? sender, EventArgs e)
    {
        if (_scheduler == null) return;  
        _scheduler.Running = false;
        EndText = "SAVE SIMULATION";
    }

    public void CreateReplay(string logPath, string mapPath)
    {
        try
        {
            _replayer = new Replay(logPath, mapPath);
            _replayer.ChangeOccurred += new EventHandler<int>(Replayer_ChangeOccured);
            OnPropertyChanged(nameof(MaxMap));
            CalculateHeight(_replayer.InitMap);
            Row = _replayer.InitMap.GetLength(0);
            Col = _replayer.InitMap.GetLength(1);
            PauseText = "\u23F8";
            CreateReplayMap();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void Replayer_ChangeOccured(object? sender, int e)
    {
        if (_replayer == null) return;
        try
        {
            Application.Current?.Dispatcher?.Invoke(() => UpdateReplayMap(_replayer.Maps[StepCount]));
        }
        catch { }
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
                    Circle = CircleColor(cell, -1, -1),
                    Square = (cell is Floor) ? Floor : Wall,
                    Id = id == null ? String.Empty : id,
                    Radius = CellSize / 2,
                });
            }
        }
    }

    private void CreateSimMap()
    {
        if (_scheduler == null) return;
        RobotNumber = "0";
        TargetLeft = "0";
        StepCount = 0;
        CreateMap(_scheduler.Map);
        foreach (CellState cell in Cells)
        {
            cell.CellClicked += new EventHandler(Cell_CellClicked);
        }
    }

    private void CreateReplayMap()
    {
        if (_replayer == null) return;
        StepCount = 0;
        CreateMap(_replayer.InitMap);
    }

    private void Cell_CellClicked(object? sender, EventArgs c)
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
        if (ShowPath)
        {
            if (c is CellCoordinates coordinates)
            {
                Cell cell = _scheduler.Map[coordinates.X, coordinates.Y];
                if (cell is Floor floor && floor.Robot != null)
                {
                    _pathIdx = floor.Robot.Id;
                }
            }
        }
    }

    private void UpdateSimMap()
    {
        if (_scheduler == null) return;

        RobotNumber = _scheduler.RobotNum.ToString();
        TargetLeft = _scheduler.TargetNum.ToString();
        for (int i = 0; i < Cells.Count; i++)
        {
            int idx = i;
            List<(int, int)> path = _scheduler.GetRobotPath(_pathIdx >= 0 ? _pathIdx : 0);
            Cell cell = _scheduler.Map[Cells[idx].X, Cells[idx].Y]; 
            String? id = ((cell is Floor s) ? (s.Robot != null ? s.Robot.Id.ToString() : (s.Target != null ? s.Target.Id.ToString() : String.Empty)) : String.Empty);
            Cells[idx].Circle = CircleColor(cell, Cells[idx].X, Cells[idx].Y);
            Cells[idx].Square = (cell is Floor) ? (_pathIdx >= 0 && path.Contains((Cells[idx].X, Cells[idx].Y)) ? InPath : Brushes.White) : Brushes.DarkSlateGray;
            Cells[idx].Id = id == null || ((cell is Floor f) && _pathIdx >= 0 && path.Contains((Cells[idx].X, Cells[idx].Y)) && path.Count != 0 && f.Robot == null && f.Target != null && path[^1] != f.Target.Pos) ? String.Empty : id;

        }
        if (_pathIdx >= 0)
        {
            List<(int, int)> path = _scheduler.GetRobotPath(_pathIdx);
            int[][] corners = GetCorners(path);
            for (int i = 0; i < path.Count; i++)
            {
                for (int j = 0; j < Cells.Count; j++)
                {
                    if ((Cells[j].X, Cells[j].Y) == path[i])
                        Cells[j].SetCorners = corners[i];
                }
            }
        }

        StepCount = _scheduler.Step;
    }

    private int[][] GetCorners(List<(int row, int col)> path)
    {
        int[][] corners = new int[path.Count][];
        for (int i = 0; i < path.Count; i++)
        {
            corners[i] = [1, 1, 1, 1];
            if (i != 0 && path[i - 1] != path[i])
            {
                if (path[i].row > path[i - 1].row)
                    corners[i] = [0, 0, .. corners[i][2..]];
                else if (path[i].row < path[i - 1].row)
                    corners[i] = [.. corners[i][..2], 0, 0];
                else if (path[i].col > path[i - 1].col)
                    corners[i] = [0, .. corners[i][1..3], 0];
                else if (path[i].col < path[i - 1].col)
                    corners[i] = [corners[i][0], 0, 0, corners[i][3]];
            }
            if (i != path.Count - 1 && path[i + 1] != path[i])
            {
                if (path[i].row > path[i + 1].row)
                    corners[i] = [0, 0, .. corners[i][2..]];
                else if (path[i].row < path[i + 1].row)
                    corners[i] = [.. corners[i][..2], 0, 0];
                else if (path[i].col > path[i + 1].col)
                    corners[i] = [0, .. corners[i][1..3], 0];
                else if (path[i].col < path[i + 1].col)
                    corners[i] = [corners[i][0], 0, 0, corners[i][3]];
            }
        }
        if (path.Count > 1)
        {
            for (int i = 1; i < path.Count; i++)
            {
                if (path[i] == path[i - 1])
                {
                    int[] union = [
                        corners[i][0] & corners[i - 1][0],
                        corners[i][1] & corners[i - 1][1],
                        corners[i][2] & corners[i - 1][2],
                        corners[i][3] & corners[i - 1][3],
                    ];
                    Array.Copy(union, corners[i], 4);
                    Array.Copy(union, corners[i - 1], 4);
                }
            }
        }

        return corners;
    }

    private void UpdateReplayMap(int[,] map)
    {
        if (_replayer == null || map == null) return;

        for (int i = 0; i < Cells.Count; i++)
        {
            int idx = i;
            int x = Cells[idx].X; int y = Cells[idx].Y;
            Cells[idx].Id = map[x, y] < -1 ? Math.Abs(map[x, y] + 2).ToString() : (map[x, y] > 1 ? ((map[x, y]  / 10) - 2).ToString() : String.Empty);
            Cells[idx].Square = map[x, y] == 0 ? Wall : Floor;
            Cells[idx].Circle = map[x, y] == 0 ? Wall : (map[x, y] == 1 ? Floor : (map[x, y] < 0 ? Target :
                (map[x, y] % 10 == 0 ? North : (map[x, y] % 10 == 1 ? East : (map[x, y] % 10 == 2 ? South : West)))));
        }
        StepCount = _replayer.Step;
    }

    private LinearGradientBrush CircleColor(Cell cell, int x, int y)
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
            if (x >= 0 && y >= 0 && _pathIdx >= 0)
            {
                if (_scheduler != null && _scheduler.GetRobotPath(_pathIdx).Contains((x, y)))
                {
                    return InPath;
                }
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


    public void ReplaySLider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (sender is Slider slider)
            StepMethod((int)slider.Value);
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
        if (_replayer == null) return;
        Task.Run(() => _replayer.Start());
    }

    private void OnExitApp()
    {
        ExitApp?.Invoke(this, EventArgs.Empty);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
