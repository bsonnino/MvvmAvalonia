using Avalonia.Controls;

namespace MvvmAvalonia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.MainVM;
        }
    }
}