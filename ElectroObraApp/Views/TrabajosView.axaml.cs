using Avalonia.Controls;
using ElectroObraApp.ViewModels;
using ElectroObraApp.Application.DTOs;

namespace ElectroObraApp.Views;

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

