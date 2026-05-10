using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.ViewModels;

namespace ElectroObraApp.Views;

public partial class MovimientosView : UserControl
{
    public MovimientosView()
    {
        InitializeComponent();
        
        DataContextChanged += (s, e) =>
        {
            if (DataContext is MovimientosViewModel vm)
            {
                vm.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(MovimientosViewModel.IsEditing) && vm.IsEditing)
                    {
                        var overlay = this.FindControl<Border>("EditOverlay");
                        overlay?.Focus();
                    }
                };
            }
        };
    }

    private void OnDataGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MovimientosViewModel vm && sender is DataGrid dg && dg.SelectedItem is MovimientoDto dto)
        {
            vm.EditCommand.Execute(dto);
        }
    }
}

