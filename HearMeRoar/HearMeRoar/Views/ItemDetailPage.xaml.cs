using HearMeRoar.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace HearMeRoar.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}