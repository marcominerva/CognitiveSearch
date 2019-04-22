using Search42.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Search42.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel => ViewModelLocator.Current.MainViewModel;

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
