using Avalonia.Controls;
using ProyectoPablito.ViewModels;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Views;

public partial class TrabajosView : UserControl
{
    public TrabajosView()
    {
        InitializeComponent();
    }

    private void OnDataGridDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is TrabajosViewModel vm && sender is DataGrid dg && dg.SelectedItem is TrabajoDto dto)
        {
            vm.EditCommand.Execute(dto);
        }
    }
}
