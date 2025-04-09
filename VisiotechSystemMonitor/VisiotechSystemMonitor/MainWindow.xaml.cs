using System.Windows;
using VisiotechSystemMonitor.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace VisiotechSystemMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Obtener la instancia de MainViewModel desde el contenedor de servicios
            var viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
            DataContext = viewModel;
        }
    }
}