using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace Warehouse_Simulation_WPF.ViewModel;



public class DelegateCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }


    public DelegateCommand(Action<object?> execute) : this(null, execute) { }
    public DelegateCommand(Predicate<object?>? canExecute, Action<object?> execute)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        _canExecute = canExecute;
    }


    public bool CanExecute(object? parameter) => _canExecute is null || _canExecute(parameter);

    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) throw new InvalidOperationException();
            _execute(parameter);
    }
}
