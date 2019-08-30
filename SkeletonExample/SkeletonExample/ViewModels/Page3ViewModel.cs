using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SkeletonExample.Models;
using Xamarin.Forms;

namespace SkeletonExample.ViewModels
{
    public class Page3ViewModel : BaseViewModel
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
                Description = "Mono is a software platform designed to allow developers to easily create cross platform applications part of the .NET Foundation."
            };

            this.IsBusy = true;
            await Task.Delay(2000);
            this.IsBusy = false;

            this.Item = new Item
            {
                Title = "Mono",
                Subtitle = "Cross platform, open source .NET framework",
                Image = "monkey.png",
                Description = "Mono is a software platform designed to allow developers to easily create cross platform applications part of the .NET Foundation."
            };
        }
    }
}
