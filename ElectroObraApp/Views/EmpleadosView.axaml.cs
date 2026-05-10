using Avalonia.Controls;
using ElectroObraApp.ViewModels;
using ElectroObraApp.Application.DTOs;

namespace ElectroObraApp.Views;

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

