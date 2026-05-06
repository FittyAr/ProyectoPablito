using CommunityToolkit.Mvvm.ComponentModel;

namespace ProyectoPablito.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Tablero";
}
