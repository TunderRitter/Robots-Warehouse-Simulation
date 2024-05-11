using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace Warehouse_Simulation_WPF.ViewModel;

/// <summary>
/// Class that represents the delegate command.
/// </summary>
public class DelegateCommand : ICommand
{
    #region Fields
    /// <summary>
    /// Action that represents the execute method.
    /// </summary>
    private readonly Action<object?> _execute;
    /// <summary>
    /// Predicate that represents the can execute method.
    /// </summary>
    private readonly Predicate<object?>? _canExecute;
    #endregion

    #region Events
    /// <summary>
    /// Event that represents the can execute changed.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
    /// </summary>
    /// <param name="execute"></param>
    public DelegateCommand(Action<object?> execute) : this(null, execute) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
    /// </summary>
    /// <param name="canExecute"></param>
    /// <param name="execute"></param>
    public DelegateCommand(Predicate<object?>? canExecute, Action<object?> execute)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>
    /// Method that checks if the command can be executed.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns>Boolean whether the command can be executed.</returns>
    public bool CanExecute(object? parameter) => _canExecute is null || _canExecute(parameter);

    /// <summary>
    /// Method that executes the command.
    /// </summary>
    /// <param name="parameter"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) throw new InvalidOperationException();
            _execute(parameter);
    }
    #endregion
}
