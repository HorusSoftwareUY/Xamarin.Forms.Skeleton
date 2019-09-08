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
            var now = DateTime.Now.ToShortDateString();

            this.Items = new ObservableCollection<Item>(new List<Item> {
                new Item
                {
                    Title = "x",
                    Subtitle = now,
                    IsBusy = true
                },
                new Item
                {
                    Title = "x",
                    Subtitle = now,
                    IsBusy = true
                },
                new Item
                {
                    Title = "x",
                    Subtitle = now,
                    IsBusy = true
                },
                new Item
                {
                    Title = "x",
                    Subtitle = now,
                    IsBusy = true
                },
            });

            this.IsBusy = true;
            await Task.Delay(2500);
            this.IsBusy = false;

            this.Items = new ObservableCollection<Item>(new List<Item> {
                new Item
                {
                    Title = "Landscape 1",
                    Subtitle = DateTime.Now.ToShortDateString(),
                    Image = "image1.jpg",
                },
                new Item
                {
                    Title = "Landscape 2",
                    Subtitle = DateTime.Now.AddDays(-2).ToShortDateString(),
                    Image = "image2.jpg",
                },
                new Item
                {
                    Title = "Landscape 3",
                    Subtitle = DateTime.Now.AddDays(-3).ToShortDateString(),
                    Image = "image4.jpg",
                },
                new Item
                {
                    Title = "Landscape 4",
                    Subtitle = DateTime.Now.AddDays(-5).ToShortDateString(),
                    Image = "image3.jpg",
                },
            });
        }
    }
}
