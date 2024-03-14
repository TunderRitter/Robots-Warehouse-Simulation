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

        private Color _square;
        public Color Square
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

        private Color _circle;
        public Color Circle
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

        private int _id;
        public int Id
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




        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
