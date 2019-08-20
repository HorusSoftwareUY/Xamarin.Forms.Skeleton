using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SkeletonExample.Models;
using Xamarin.Forms;

namespace SkeletonExample.ViewModels
{
    public class Page2ViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }

        public ICommand LoadCommand => new Command(OnLoadCommand);

        public Page2ViewModel()
        {
            this.Items = new ObservableCollection<Item>();
        }

        private void OnLoadCommand()
        {
            IsBusy = !IsBusy;
        }
    }
}
