using SkeletonExample.ViewModels;

namespace SkeletonExample.Pages
{
    public partial class Page2 : BasePage
    {
        public Page2()
        {
            InitializeComponent();
            this.BindingContext = new Page2ViewModel();
        }
    }
}
