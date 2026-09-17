using gLAMS.WPF.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace gLAMS.WPF.Views.Pages
{
    public partial class DataPage : INavigableView<DataViewModel>
    {
        public DataViewModel ViewModel { get; }

        public DataPage(DataViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
