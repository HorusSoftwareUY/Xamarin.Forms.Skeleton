using SkeletonExample.Pages;
using System.ComponentModel;
using Xamarin.Forms;

namespace SkeletonExample
{
    [DesignTimeVisible(false)]
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.Children.Add(new Page1());
            this.Children.Add(new Page2());
            this.Children.Add(new Page3());
            this.Children.Add(new Page4());
            this.Children.Add(new Page5());
            this.Children.Add(new Page6());
        }
    }
}
