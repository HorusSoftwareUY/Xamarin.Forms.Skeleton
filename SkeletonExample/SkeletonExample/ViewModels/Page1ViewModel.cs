using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SkeletonExample.Models;
using Xamarin.Forms;

namespace SkeletonExample.ViewModels
{
    public class Page1ViewModel : BaseViewModel
    {
        private ObservableCollection<Item> items;

        public ObservableCollection<Item> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }

        public ICommand LoadCommand => new Command(OnLoadCommand);

        private async void OnLoadCommand()
        {
            this.Items = new ObservableCollection<Item>(new List<Item> {
                new Item
                {
                    Title = "x",
                    Subtitle = "x",
                    IsBusy = true
                },
                new Item
                {
                    Title = "x",
                    Subtitle = "x",
                    IsBusy = true
                },
                new Item
                {
                    Title = "x",
                    Subtitle = "x",
                    IsBusy = true
                },
                new Item
                {
                    Title = "x",
                    Subtitle = "x",
                    IsBusy = true
                },
            });

            this.IsBusy = true;
            await Task.Delay(2000);
            this.IsBusy = false;

            this.Items = new ObservableCollection<Item>(new List<Item> {
                new Item
                {
                    Title = "Xamarin",
                    Subtitle = "Open-source mobile app platform for .NET",
                    Image = "xamarin.png",
                },
                new Item
                {
                    Title = "Mono",
                    Subtitle = "Cross platform, open source .NET framework",
                    Image = "monkey.png",
                },
                new Item
                {
                    Title = "Xamarin.Forms.Skeleton",
                    Subtitle = "The new loading approach for cool apps!",
                    Image = "icon.png",
                },
                new Item
                {
                    Title = "Horus",
                    Subtitle = "Mobile Development Studio",
                    Image = "horus.png",
                },
            });
        }
    }
}
