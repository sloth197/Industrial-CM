using System.Windows;
using IndustrialIoTManager.ViewModel;

namespace IndustrialIoTManager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
