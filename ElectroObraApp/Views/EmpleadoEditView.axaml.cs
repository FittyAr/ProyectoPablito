using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ElectroObraApp.Views;

public partial class EmpleadoEditView : UserControl
{
    public EmpleadoEditView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

