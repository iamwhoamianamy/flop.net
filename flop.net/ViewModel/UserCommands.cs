using System;

namespace flop.net.ViewModel;

public class UserCommands
{
    private Action<object> execute;
    private Action<object> unexecute;
    public UserCommands(Action<object> execute, Action<object> unexecute)
    {
        this.execute = execute;
        this.unexecute = unexecute;
    }

    public RelayCommand Execute => new (execute);

    public RelayCommand UnExecute => new (unexecute);
}