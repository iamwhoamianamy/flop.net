using System.ComponentModel;
using System.Runtime.CompilerServices;
using flop.net.Annotations;

namespace flop.net.ViewModel;

public class MainWindowVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}