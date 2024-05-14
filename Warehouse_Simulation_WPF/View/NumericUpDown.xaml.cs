using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Warehouse_Simulation_WPF.View;

/// <summary>
/// NumericUpDown component.
/// </summary>
public partial class NumericUpDown : UserControl
{
    /// <summary>
    /// Selection state of the input text.
    /// </summary>
    private bool _selected = false;

    private int _min = 0;
    /// <summary>
    /// Minimum value.
    /// </summary>
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

    private int _max = 1_000_000;
    /// <summary>
    /// Maximum value.
    /// </summary>
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

    /// <summary>
    /// Default value.
    /// </summary>
    public int Default
    {
        get => (int)GetValue(DefaultProperty);
        set => SetValue(DefaultProperty, value);
    }
    public static readonly DependencyProperty DefaultProperty =
        DependencyProperty.Register("Default", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0));

    /// <summary>
    /// Current value.
    /// </summary>
    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string), typeof(NumericUpDown), new PropertyMetadata("0"));

    /// <summary>
    /// Command for getting and setting the value.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(NumericUpDown));


    /// <summary>
    /// Creates <see cref="NumericUpDown"/> object.
    /// </summary>
    public NumericUpDown()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Increments the value.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void IncrementButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(numericTextBox.Text, out int value) && value < Max)
            numericTextBox.Text = (++value).ToString();
    }

    /// <summary>
    /// Decrements the value.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DecrementButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(numericTextBox.Text, out int value) && value > Min)
            numericTextBox.Text = (--value).ToString();
    }

    /// <summary>
    /// Method handling invalid input.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Method handling invalid input.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
            e.Handled = true;
    }

    /// <summary>
    /// Method handling selection state.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (numericTextBox.SelectionStart == 0 && numericTextBox.SelectionLength > 0)
            _selected = true;
    }

    /// <summary>
    /// Method handling focus state.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        numericTextBox.SelectAll();
        _selected = true;
    }

    /// <summary>
    /// Method handling focus state.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!numericTextBox.IsKeyboardFocusWithin)
        {
            numericTextBox.Focus();
            e.Handled = true;
        }
    }

    /// <summary>
    /// Method handling focus state.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(numericTextBox.Text, out int value))
            numericTextBox.Text = Math.Clamp(value, Min, Max).ToString();
        else if (numericTextBox.Text == "")
            numericTextBox.Text = Default.ToString();
        else
            numericTextBox.Text = Max.ToString();
    }

    /// <summary>
    /// Executes command.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e) => Command?.Execute(numericTextBox.Text);
}