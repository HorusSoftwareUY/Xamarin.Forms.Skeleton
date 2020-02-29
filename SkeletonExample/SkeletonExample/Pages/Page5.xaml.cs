using SkeletonExample.ViewModels;

namespace SkeletonExample.Pages
{
    public partial class Page5 : BasePage
    {
        public Page5()
        {
            InitializeComponent();
            BindingContext = new Page5ViewModel();
        }
    }
}