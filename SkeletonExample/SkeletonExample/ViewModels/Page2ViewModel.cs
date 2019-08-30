using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SkeletonExample.Models;
using Xamarin.Forms;

namespace SkeletonExample.ViewModels
{
    public class Page2ViewModel : BaseViewModel
    {
        private Item item;

        public Item Item
        {
            get { return item; }
            set { SetProperty(ref item, value); }
        }

        public ICommand LoadCommand => new Command(OnLoadCommand);

        private async void OnLoadCommand()
        {
            this.Item = new Item
            {
                Title = "x",
                Subtitle = "x",
                Image = null,
                Description = "Modern, performant mobile apps with .NET and C#. Use.NET and C# to create native apps for the billions of Android, iPhone, iPad, Mac, and Windows devices around the world. Add to that sharing code with any .NET app, empowering you to be more productive and build apps faster than ever across all platforms."
            };

            this.IsBusy = true;
            await Task.Delay(2000);
            this.IsBusy = false;

            this.Item = new Item
            {
                Title = "Xamarin",
                Subtitle = "Open-source mobile app platform for .NET",
                Image = "xamarin.png",
                Description = "Modern, performant mobile apps with .NET and C#. Use.NET and C# to create native apps for the billions of Android, iPhone, iPad, Mac, and Windows devices around the world. Add to that sharing code with any .NET app, empowering you to be more productive and build apps faster than ever across all platforms."
            };
        }
    }
}
