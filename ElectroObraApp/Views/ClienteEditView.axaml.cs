using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ElectroObraApp.Views;

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

