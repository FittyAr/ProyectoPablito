using Avalonia.Controls;
using Avalonia.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.ViewModels;

namespace ProyectoPablito.Views;

public partial class MovimientosView : UserControl
{
    public MovimientosView()
    {
        InitializeComponent();
    }

    private void OnDataGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MovimientosViewModel vm && sender is DataGrid dg && dg.SelectedItem is MovimientoDto dto)
        {
            vm.EditCommand.Execute(dto);
        }
    }
}
