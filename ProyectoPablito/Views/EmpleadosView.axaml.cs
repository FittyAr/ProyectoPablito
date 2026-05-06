using Avalonia.Controls;
using ProyectoPablito.ViewModels;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Views;

public partial class EmpleadosView : UserControl
{
    public EmpleadosView()
    {
        InitializeComponent();
    }

    private void OnDataGridDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is EmpleadosViewModel vm && sender is DataGrid dg && dg.SelectedItem is EmpleadoDto dto)
        {
            vm.EditCommand.Execute(dto);
        }
    }
}
