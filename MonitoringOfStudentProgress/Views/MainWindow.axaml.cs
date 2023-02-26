using Avalonia.Controls;
using JournalOfStudents.ViewModels;

namespace JournalOfStudents.Views {
    public partial class MainWindow: Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
