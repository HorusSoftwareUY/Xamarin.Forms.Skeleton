using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SkeletonExample.Models;
using Xamarin.Forms;

namespace SkeletonExample.ViewModels
{
    public class Page1ViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }

        public ICommand LoadCommand => new Command(OnLoadCommand);

        public Page1ViewModel()
        {
            this.Items = new ObservableCollection<Item>();
        }

        private async void OnLoadCommand()
        {
            this.IsBusy = true;
            await Task.Delay(2000);
            this.IsBusy = false;
        }
    }
}
