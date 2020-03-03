using SkeletonExample.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkeletonExample.Pages
{
    public partial class Page4 : BasePage
    {
        public Page4()
        {
            InitializeComponent();
            this.BindingContext = new Page4ViewModel();
        }
    }
}