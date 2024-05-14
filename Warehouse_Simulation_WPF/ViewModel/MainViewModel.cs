using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Warehouse_Simulation_Model.Model;
using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_WPF.ViewModel;

/// <summary>
/// Class that represents the main view model.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    #region Fields
    /// <summary>
    /// Schduler object.
    /// </summary>
    Scheduler? _scheduler;
    /// <summary>
    /// Replay object.
    /// </summary>
    Replay? _replayer;

    /// <summary>
    /// Brush for the south direction.
    /// </summary>
    private readonly LinearGradientBrush South = new(Colors.LightCyan, Colors.DarkCyan, 90.0);
    /// <summary>
    /// Brush for the north direction.
    /// </summary>
    private readonly LinearGradientBrush North = new(Colors.DarkCyan, Colors.LightCyan, 90.0);
    /// <summary>
    /// Brush for the east direction.
    /// </summary>
    private readonly LinearGradientBrush East = new(Colors.LightCyan, Colors.DarkCyan, 0.0);
    /// <summary>
    /// Brush for the west direction.
    /// </summary>
    private readonly LinearGradientBrush West = new(Colors.DarkCyan, Colors.LightCyan, 0.0);
    /// <summary>
    /// Brush for the targets.
    /// </summary>
    private readonly LinearGradientBrush Target = new(Colors.Salmon, Colors.Salmon, 0.0);
    /// <summary>
    /// Brush for the inactive targets.
    /// </summary>
    private readonly LinearGradientBrush InactiveTarget = new(Colors.LightGray, Colors.LightGray, 0.0);
    /// <summary>
    /// Brush for the walls.
    /// </summary>
    private readonly LinearGradientBrush Wall = new(Colors.DarkSlateGray, Colors.DarkSlateGray, 0.0);
    /// <summary>
    /// Brush for the floor.
    /// </summary>
    private readonly LinearGradientBrush Floor = new(Colors.White, Colors.White, 0.0);
    /// <summary>
    /// Brudh for the path shown.
    /// </summary>
    private readonly LinearGradientBrush InPath = new(Colors.PaleGreen, Colors.PaleGreen, 0.0);
    #endregion

    #region Properties
    private int _row;
    /// <summary>
    /// Property that represents the row of the map.
    /// </summary>
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
    /// <summary>
    /// Property that represents the column of the map.
    /// </summary>
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
    /// <summary>
    /// Property that represents the height of the window.
    /// </summary>
    public int HeightOfWIndow { get; set; }

    private int _mapHeight;
    /// <summary>
    /// Property that represents the height of the map.
    /// </summary>
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
    /// <summary>
    /// Property that represents the width of the map.
    /// </summary>
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

    /// <summary>
    /// Integer that represents the height of the scroll view.
    /// </summary>
    public int ScrollViewHeight => MapHeight + 20;
    /// <summary>
    /// Integer that represents the width of the scroll view.
    /// </summary>
    public int ScrollViewWidth => MapWidth + 20;

    private int _cellSize;
    /// <summary>
    /// Property that represents the size of a cell.
    /// </summary>
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
    /// <summary>
    /// Integer that represents the size of a circle.
    /// </summary>
    public int CircleSize => CellSize - 10;

    private int _zoomValue;
    /// <summary>
    /// Property that represents the zoom value.
    /// </summary>
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
    /// <summary>
    /// Property that represents the int value.
    /// </summary>
    public string IntValue
    {
        get => _intValue.ToString();
        set
        {
            if (int.TryParse(value, out int val) && val != _intValue)
            {
                _intValue = val;
                OnPropertyChanged();
            }
        }
    }

    private int _stepValue;
    /// <summary>
    /// Property that represents the step value.
    /// </summary>
    public string StepValue
    {
        get => _stepValue.ToString();
        set
        {
            if (int.TryParse(value, out int val) && val != _stepValue)
            {
                _stepValue = val;
                OnPropertyChanged();
            }
        }
    }

    private bool _canOrder;
    /// <summary>
    /// Property that represents whether the user can order.
    /// </summary>
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
    /// <summary>
    /// Property that represents whether the path is shown.
    /// </summary>
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
                if (value)
                {
                    _pathIdx = 0;
                }
                _showPath = value;
                OnPropertyChanged();
            }
        }
    }

    private int _stepCount;
    /// <summary>
    /// Property that represents the step count.
    /// </summary>
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
    /// <summary>
    /// Property that represents the robot number.
    /// </summary>
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
    /// <summary>
    /// Property that represents the number of targets left.
    /// </summary>
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

    /// <summary>
    /// Integer that represents the max number of maps.
    /// </summary>
    public int MaxMap => _replayer?.MaxStep ?? 0;

    private string _pauseText;
    /// <summary>
    /// Property that represents the pause text (pause symbol).
    /// </summary>
    public string PauseText
    {
        get => _pauseText;
        set
        {
            if (_pauseText != value)
            {
                _pauseText = value;
                OnPropertyChanged();
            }
        }
    }
    private string _endText;
    /// <summary>
    /// Property that represents the end text (end symbol).
    /// </summary>
    public string EndText
    {
        get => _endText;
        set
        {
            if (_endText != value)
            {
                _endText = value;
                OnPropertyChanged();
            }
        }
    }
    /// <summary>
    /// Integer that represents the path index.
    /// </summary>
    private int _pathIdx;

    /// <summary>
    /// ObservableCollection of cell states (map representetive).
    /// </summary>
    public ObservableCollection<CellState> Cells { get; private set; }

    /// <summary>
    /// Command for starting a new simulation.
    /// </summary>
    public DelegateCommand NewSimulation { get; private set; }
    /// <summary>
    /// Command for loading a replay.
    /// </summary>
    public DelegateCommand LoadReplay { get; private set; }
    /// <summary>
    /// Command for starting a simulation.
    /// </summary>
    public DelegateCommand StartSim { get; private set; }
    /// <summary>
    /// Command for starting a replay.
    /// </summary>
    public DelegateCommand StartReplay { get; private set; }
    /// <summary>
    /// Command for exiting the application.
    /// </summary>
    public DelegateCommand Exit { get; private set; }
    /// <summary>
    /// Command for zooming.
    /// </summary>
    public DelegateCommand Zoom { get; private set; }
    /// <summary>
    /// Command for stepping in the replay.
    /// </summary>
    public DelegateCommand StepCommand { get; init; }
    /// <summary>
    /// Command for changing the int value.
    /// </summary>
    public DelegateCommand IntCommand { get; init; }
    /// <summary>
    /// Command for going back to the menu.
    /// </summary>
    public DelegateCommand BackToMenu { get; init; }
    /// <summary>
    /// Command for stepping forward in the replay.
    /// </summary>
    public DelegateCommand StepFwd { get; init; }
    /// <summary>
    /// Command for stepping back in the replay.
    /// </summary>
    public DelegateCommand StepBack { get; init; }
    /// <summary>
    /// Command for playing or pausing the replay.
    /// </summary>
    public DelegateCommand PlayPause { get; init; }
    /// <summary>
    /// Command for ending the simulation.
    /// </summary>
    public DelegateCommand EndCommand { get; init; }
    /// <summary>
    /// Command for slowing down the replay.
    /// </summary>
    public DelegateCommand Slow { get; init; }
    /// <summary>
    /// Command for speeding up the replay.
    /// </summary>
    public DelegateCommand Fast { get; init; }
    /// <summary>
    /// Command for switching to the next robot's path
    /// </summary>
    public DelegateCommand PathNumberInc { get; init; }
    /// <summary>
    /// Command for switching to the previous robot's path
    /// </summary>
    public DelegateCommand PathNumberDec { get; init; }

    #endregion

    #region Events
    /// <summary>
    /// Event that represents the property changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// Event that represents the new simulation started.
    /// </summary>
    public event EventHandler<(string, string)>? NewSimulationStarted;
    /// <summary>
    /// Event that represents the replay.
    /// </summary>
    public event EventHandler<(string, string)>? Replay;
    /// <summary>
    /// Event that represents the save log.
    /// </summary>
    public event EventHandler? SaveLog;
    /// <summary>
    /// Event that represents the exit app.
    /// </summary>
    public event EventHandler? ExitApp;
    #endregion

    #region Public Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
    {
        ZoomValue = 1;
        IntValue = "1000";
        StepValue = "1000";
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
        PathNumberInc = new DelegateCommand(param => SwitchPath(1));
        PathNumberDec = new DelegateCommand(param => SwitchPath(-1));

        Cells = [];
        _pauseText = "";
        _endText = "";
    }

    /// <summary>
    /// Method that creates the scheduler.
    /// </summary>
    /// <param name="path">The config file's path</param>
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

    /// <summary>
    /// Method that saves the log file.
    /// </summary>
    /// <param name="path">The save file's path</param>
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

    /// <summary>
    /// Method that creates the replay.
    /// </summary>
    /// <param name="logPath">The log file's path</param>
    /// <param name="mapPath">The map file's path</param>
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

    /// <summary>
    /// Method that updates the slider value.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void ReplaySLider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (sender is Slider slider)
            StepMethod((int)slider.Value);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Method that speeds up the replay.
    /// </summary>
    private void FastReplay()
    {
        if (_replayer == null) return;
        _replayer.FasterSpeed();
    }

    /// <summary>
    /// Method that slows down the replay.
    /// </summary>
    private void SlowReplay()
    {
        if (_replayer == null) return;
        _replayer.SlowerSpeed();
    }

    /// <summary>
    /// Method that ends the simulation.
    /// </summary>
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

    /// <summary>
    /// Method that plays or pauses the replay.
    /// </summary>
    private void PlayPauseMethod()
    {
        if (_replayer == null) return;
        if (_replayer.Paused)
        {
            _replayer.Play();
            PauseText = "\u23F8";
        }
        else
        {
            _replayer.Pause();
            PauseText = "\u25B6";
        }
    }

    /// <summary>
    /// Method that updates the map.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Scheduler_ChangeOccurred(object? sender, EventArgs e)
    {
        if (_scheduler == null) return;
        try
        {
            Application.Current?.Dispatcher?.Invoke(UpdateSimMap);
        }
        catch { }
    }

    /// <summary>
    /// Method that updates data when going back to the menu.
    /// </summary>
    /// <param name="parameter"></param>
    private void OnBackToMenu(object? parameter)
    {
        Cells.Clear();
        IntValue = "1000";
        StepValue = "1000";
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

    /// <summary>
    /// Method that skips to a certain step in the replay.
    /// </summary>
    /// <param name="parameter"></param>
    private void StepMethod(int parameter)
    {
        if (_replayer == null) return;
        _replayer.SkipTo(parameter);
    }

    /// <summary>
    /// Method that updates the map when the simulation is finished.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Scheduler_SimFinished(object? sender, EventArgs e)
    {
        if (_scheduler == null) return;
        _scheduler.Running = false;
        _pathIdx = -1;
        UpdateSimMap();
        EndText = "SAVE SIMULATION";
    }

   /// <summary>
   /// Method that updates the replay map.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    private void Replayer_ChangeOccured(object? sender, int e)
    {
        if (_replayer == null) return;
        try
        {
            Application.Current?.Dispatcher?.Invoke(() => UpdateReplayMap(_replayer.Maps[StepCount]));
        }
        catch { }
    }

    /// <summary>
    /// Method that calculates the height and width of the map and the cells.
    /// </summary>
    /// <param name="map"></param>
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

    /// <summary>
    /// Method that creates the map.
    /// </summary>
    /// <param name="map"></param>
    private void CreateMap(Cell[,] map)
    {
        Cells.Clear();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                Cell cell = map[i, j];
                string? id = (cell is Floor s) ? (s.Robot != null ? s.Robot.Id.ToString() : (s.Target != null ? s.Target.Id.ToString() : "")) : "";

                Cells.Add(new CellState
                {
                    X = i,
                    Y = j,
                    Circle = CircleColor(cell, -1, -1),
                    Square = (cell is Floor) ? Floor : Wall,
                    Id = id ?? "",
                    Radius = CellSize / 2
                });
            }
        }
    }

    /// <summary>
    /// Method that creates the simulation map.
    /// </summary>
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

    /// <summary>
    /// Method that creates the replay map.
    /// </summary>
    private void CreateReplayMap()
    {
        if (_replayer == null) return;
        RobotNumber = "0";
        TargetLeft = "0";
        StepCount = 0;
        CreateMap(_replayer.InitMap);
    }

    /// <summary>
    /// Method that handles the cell click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="c"></param>
    private void Cell_CellClicked(object? sender, EventArgs c)
    {
        if (_scheduler == null) return;
        if (CanOrder)
        {
            if (c is CellCoordinates coordinates)
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
    /// <summary>
    /// Method that handles the switching between paths
    /// </summary>
    /// <param name="n"></param>
    private void SwitchPath(int n)
    {
        if (ShowPath)
        {
            if (_pathIdx == _robotNumber-1 && n == 1)
            {
                _pathIdx = 0;
            }
            else if (_pathIdx == 0 && n == -1)
            {
                _pathIdx = _robotNumber - 1;
            }
            else
            {
                _pathIdx += n;
            }

        }
    }
    /// <summary>
    /// Method that updates the simulation map
    /// </summary>
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
            string? id = cell is Floor s
                ? (s.Robot != null
                    ? s.Robot.Id.ToString()
                    : (s.Target != null ? s.Target.Id.ToString() : ""))
                : "";
            Cells[idx].Circle = CircleColor(cell, Cells[idx].X, Cells[idx].Y);
            Cells[idx].Square = cell is Floor
                ? (_pathIdx >= 0 && path.Contains((Cells[idx].X, Cells[idx].Y))
                    ? InPath
                    : Floor)
                : Wall;
            Cells[idx].Id = id == null || (cell is Floor f
                && _pathIdx >= 0
                && path.Contains((Cells[idx].X, Cells[idx].Y))
                && path.Count != 0
                && f.Robot == null
                && f.Target != null
                && path[^1] != f.Target.Pos)
                    ? ""
                    : id;

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

        if (_scheduler.Running)
            StepCount = _scheduler.Step;
    }

    /// <summary>
    /// Method that gets the rounded corners of the shown path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>Which corners should be rounded for the shown path.</returns>
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

    /// <summary>
    /// Method that updates the replay map.
    /// </summary>
    /// <param name="map"></param>
    private void UpdateReplayMap(int[,] map)
    {
        if (_replayer == null || map == null) return;

        RobotNumber = _replayer.RobotNum.ToString();
        TargetLeft = _replayer.TargetNum.ToString();
        for (int i = 0; i < Cells.Count; i++)
        {
            int x = Cells[i].X; int y = Cells[i].Y;
            Cells[i].Id = map[x, y] < -1
                ? (-map[x, y] - 2).ToString()
                : map[x, y] > 1
                    ? (map[x, y] / 10 - 2).ToString()
                    : "";
            Cells[i].Square = map[x, y] == 0 ? Wall : Floor;
            Cells[i].Circle = map[x, y] switch
            {
                0 => Wall,
                1 => Floor,
                -1 => InactiveTarget,
                < -1 => Target,
                _ => (map[x, y] % 10) switch
                {
                    0 => North,
                    1 => East,
                    2 => South,
                    _ => West,
                },
            };
        }
        StepCount = _replayer.Step;
    }

    /// <summary>
    /// Method that calculates the color of the circle.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>The color of the circle</returns>
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
                if (floor.Target.Active && _scheduler != null) return Target;
                return InactiveTarget;
            }

            return Floor;
        }
        return Floor;
    }

    /// <summary>
    /// Method that zooms in or out.
    /// </summary>
    /// <param name="parameter"></param>
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

    /// <summary>
    /// Method that handles starting the simulation.
    /// </summary>
    private void OnSimStart()
    {
        if (_scheduler == null) return;
        _scheduler.MaxSteps = _stepValue;
        _scheduler.TimeLimit = _intValue;
        Task.Run(() => _scheduler.Schedule());
    }

    /// <summary>
    /// Method that handles asking for the config file.
    /// </summary>
    private void OnNewSimulation()
    {
        NewSimulationStarted?.Invoke(this, ("Choose config file", "config"));
    }

    /// <summary>
    /// Method that handles asking for the log file.
    /// </summary>
    private void OnReplay()
    {
        Replay?.Invoke(this, ("Choose log file", "log"));
    }

    /// <summary>
    /// Method that handles starting the replay.
    /// </summary>
    private void OnReplayStart()
    {
        if (_replayer == null) return;
        Task.Run(() => _replayer.Start());
    }

    /// <summary>
    /// Method that handles exiting the application.
    /// </summary>
    private void OnExitApp()
    {
        ExitApp?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Method that handes the propertychanged event.
    /// </summary>
    /// <param name="propertyName"></param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion
}
