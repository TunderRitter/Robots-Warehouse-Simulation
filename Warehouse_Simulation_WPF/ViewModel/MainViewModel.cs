﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Warehouse_Simulation_Model.Model;

namespace Warehouse_Simulation_WPF.ViewModel;


public class MainViewModel : INotifyPropertyChanged
{
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


    private String _menu = "Visible";
	public String Menu
	{
		get { return _menu; }
        set
        {
            if (_menu != value)
            {
                _menu = value;
                OnPropertyChanged(nameof(Menu));
            }
        }
    }

    private String _simulation = "Collapsed";
    public String Simulation
    {
        get { return _simulation; }
        set
        {
            if (_simulation != value)
            {
                _simulation = value;
                OnPropertyChanged(nameof(Simulation));
            }
        }
    }

    private String _replay = "Collapsed";
    public String Replay
    {
        get { return _replay; }
        set
        {
            if (_replay != value)
            {
                _replay = value;
                OnPropertyChanged(nameof(Replay));
            }
        }
    }

    private int _zoomValue;
    public int ZoomValue
    {
        get { return _zoomValue; }
        set { _zoomValue = value; OnPropertyChanged(nameof(ZoomValue)); }
    }




    public ObservableCollection<CellState> Cells { get; private set; }

    public DelegateCommand NewSimulation { get; private set; }
    public DelegateCommand LoadReplay { get; private set; }
    public DelegateCommand Exit { get; private set; }

    public DelegateCommand Zoom { get; private set; }


    public MainViewModel() 
    {
        Zoom = new DelegateCommand(ZoomMethod);
        ZoomValue = 1;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
