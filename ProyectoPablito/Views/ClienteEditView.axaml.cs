using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ProyectoPablito.Views;

public partial class ClienteEditView : UserControl
{
    public ClienteEditView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
