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
                    Title = "Skeleton Control Loading",
                    Subtitle = "What is Skeleton?",
                    Image = null,
                },
                //new Item
                //{
                //    Title = "Skeleton Control",
                //    Subtitle = "What is Skeleton?",
                //    Image = null,
                //},
                //new Item
                //{
                //    Title = "Skeleton Control",
                //    Subtitle = "What is Skeleton?",
                //    Image = null,
                //},
                //new Item
                //{
                //    Title = "Skeleton Control",
                //    Subtitle = "What is Skeleton?",
                //    Image = null,
                //},
            });

            this.IsBusy = true;
            await Task.Delay(2000);
            this.IsBusy = false;

            this.Items = new ObservableCollection<Item>(new List<Item> {
                new Item
                {
                    Title = "Skeleton Control",
                    Subtitle = "What is Skeleton?",
                    Image = "xamarin.png",
                },
                //new Item
                //{
                //    Title = "Skeleton Control",
                //    Subtitle = "What is Skeleton?",
                //    Image = "xamarin.png",
                //},
                //new Item
                //{
                //    Title = "Skeleton Control",
                //    Subtitle = "What is Skeleton?",
                //    Image = "xamarin.png",
                //},
                //new Item
                //{
                //    Title = "Skeleton Control",
                //    Subtitle = "What is Skeleton?",
                //    Image = "xamarin.png",
                //},
            });
        }
    }
}
