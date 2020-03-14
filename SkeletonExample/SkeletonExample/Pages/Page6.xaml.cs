using SkeletonExample.ViewModels;

namespace SkeletonExample.Pages
{
    public partial class Page6 : BasePage
    {
        public Page6()
        {
            InitializeComponent();
            BindingContext = new Page6ViewModel();
        }
    }
}