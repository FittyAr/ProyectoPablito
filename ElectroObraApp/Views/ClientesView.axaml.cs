using Avalonia.Controls;
using ElectroObraApp.ViewModels;
using ElectroObraApp.Application.DTOs;

namespace ElectroObraApp.Views;

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

