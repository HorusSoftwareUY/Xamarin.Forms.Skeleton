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

        protected override async void OnLoadCommandExecute()
        {
            this.Item = new Item
            {
                Title = "x",
                Subtitle = "x",
                Image = null,
                Description = "The new loading approach for cool apps in Xamarin Forms.\n\nAvailable on NuGet and support Xamarin.Forms >= 4.0.0.425677."
            };

            this.IsBusy = true;
            await Task.Delay(2500);
            this.IsBusy = false;

            this.Item = new Item
            {
                Title = "Skeleton",
                Subtitle = "Xamarin.Forms.Skeleton",
                Image = "icon.png",
                Description = "The new loading approach for cool apps in Xamarin Forms.\n\nAvailable on NuGet and support Xamarin.Forms >= 4.0.0.425677."
            };
        }
    }
}
