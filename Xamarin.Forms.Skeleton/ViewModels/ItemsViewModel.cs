using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Xamarin.Forms.Skeleton.Models;
using Xamarin.Forms.Skeleton.Views;

namespace Xamarin.Forms.Skeleton.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command LoadCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Skeleton samples";
            Items = new ObservableCollection<Item>();
            LoadCommand = new Command(async () => await ExecuteLoadCommand());

        }

        async Task ExecuteLoadCommand()
        {
            IsBusy = !IsBusy;
        }
    }
}