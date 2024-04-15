using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media;

namespace Warehouse_Simulation_WPF.ViewModel
{
    public class CellState :INotifyPropertyChanged
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

        private Brush _circle;
        public Brush Circle
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

        private String _id;
        public String Id
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
        private string _direction;

        public string Direction
        {
            get { return _direction; }
            set { _direction = value; OnPropertyChanged(nameof(Direction)); }
        }

        public CellState()
        {
            OnlineOrder = new DelegateCommand(OrderMethod);
        }
        private void OrderMethod(object? parameter)
        {
            TargetPlaced?.Invoke(this, new CellCoordinates(X, Y));
        }
        public DelegateCommand OnlineOrder { get; set; }
        public event EventHandler? TargetPlaced;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public class CellCoordinates: EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CellCoordinates(int x, int y)
        {
            X = x; Y = y;
        }
    }
}
