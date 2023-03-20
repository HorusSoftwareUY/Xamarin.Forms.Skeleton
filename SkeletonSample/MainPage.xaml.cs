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

//public partial class MainPage : ContentPage
//{
//	int count = 0;

//	public MainPage()
//	{
//		InitializeComponent();
//	}

//	private void OnCounterClicked(object sender, EventArgs e)
//	{
//		count++;

//		if (count == 1)
//			CounterBtn.Text = $"Clicked {count} time";
//		else
//			CounterBtn.Text = $"Clicked {count} times";

//		SemanticScreenReader.Announce(CounterBtn.Text);
//	}
//}