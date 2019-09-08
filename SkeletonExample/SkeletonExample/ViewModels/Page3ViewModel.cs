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
                Description = "We are a mobile development studio that is very passionate about creating quality mobile solutions.\n\nWe specialize in native and cross-platform mobile development. Beside that, we provide our clients help with UX / UI, web development and cloud computing."
            };

            this.IsBusy = true;
            await Task.Delay(2500);
            this.IsBusy = false;

            this.Item = new Item
            {
                Title = "Horus",
                Subtitle = "Mobile Development Studio",
                Image = "horus2.png",
                Description = "We are a mobile development studio that is very passionate about creating quality mobile solutions.\n\nWe specialize in native and cross-platform mobile development. Beside that, we provide our clients help with UX / UI, web development and cloud computing."
            };
        }
    }
}
