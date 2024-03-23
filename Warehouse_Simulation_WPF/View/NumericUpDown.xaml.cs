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


    private void IncrementButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(numericTextBox.Text, out int value) && value < Max)
            numericTextBox.Text = (++value).ToString();
    }

    private void DecrementButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(numericTextBox.Text, out int value) && value > Min)
            numericTextBox.Text = (--value).ToString();
    }

    private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if ((_selected || numericTextBox.Text == "") && e.Text == "0")
        {
            e.Handled = true;
            return;
        }
        if (!int.TryParse(e.Text, out _))
            e.Handled = true;
        else
            _selected = false;
    }

    private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
            e.Handled = true;
    }

    private void NumericTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (numericTextBox.SelectionStart == 0 && numericTextBox.SelectionLength > 0)
            _selected = true;
    }

    private void NumericTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        numericTextBox.SelectAll();
        _selected = true;
    }

    private void NumericTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!numericTextBox.IsKeyboardFocusWithin)
        {
            numericTextBox.Focus();
            e.Handled = true;
        }
    }

    private void NumericTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(numericTextBox.Text, out int value))
            numericTextBox.Text = Math.Clamp(value, Min, Max).ToString();
        else if (numericTextBox.Text == "")
            numericTextBox.Text = Default.ToString();
        else
            numericTextBox.Text = Max.ToString();
    }

    private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e) => Command?.Execute(numericTextBox.Text);
}