using Avalonia.Controls;
using ProyectoPablito.ViewModels;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Views;

public partial class ClientesView : UserControl
{
    public ClientesView()
    {
        InitializeComponent();
    }

    private void OnDataGridDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is ClientesViewModel vm && sender is DataGrid dg && dg.SelectedItem is ClienteDto dto)
        {
            vm.EditCommand.Execute(dto);
        }
    }
}
