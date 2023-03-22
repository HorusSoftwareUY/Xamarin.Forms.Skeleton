using SkeletonSample.Pages;

namespace SkeletonSample;

public partial class MainPage : TabbedPage
{
    public MainPage()
    {
        InitializeComponent();
        this.Children.Add(new Skeleton());
        this.Children.Add(new Beat());
        this.Children.Add(new Fade());
        this.Children.Add(new VerticalShake());
        this.Children.Add(new HorizontalShake());
        this.Children.Add(new CustomAnimation());
    }
}