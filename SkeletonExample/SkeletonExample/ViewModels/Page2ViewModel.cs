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
            this.IsBusy = true;
            await Task.Delay(2000);
            this.IsBusy = false;
        }
    }
}
