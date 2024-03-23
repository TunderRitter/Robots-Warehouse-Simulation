using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Warehouse_Simulation_WPF.View;


public partial class NumericUpDown : UserControl
{
    private bool _selected = false;

    private int _min = 0;
    public int Min
    {
        get => _min;
        set
        {
            _min = value;
            int defValue = Math.Max(value, Default);
            if (defValue != Default)
                Default = defValue;
        }
    }

    private int _max = 100;
    public int Max
    {
        get => _max;
        set
        {
            _max = value;
            int defValue = Math.Min(value, Default);
            if (defValue != Default)
                Default = defValue;
        }
    }


    public int Default
    {
        get => (int)GetValue(DefaultProperty);
        set => SetValue(DefaultProperty, value);
    }
    public static readonly DependencyProperty DefaultProperty =
        DependencyProperty.Register("Default", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0));

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string), typeof(NumericUpDown), new PropertyMetadata("0"));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(NumericUpDown));



    public NumericUpDown()
    {
        InitializeComponent();
    }
}