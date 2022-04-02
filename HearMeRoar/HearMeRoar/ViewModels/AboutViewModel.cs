using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HearMeRoar.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            GetContacts = new Command(async() => GetMyContacts()) ;
        }

        public ICommand GetContacts { get; }

        private async void GetMyContacts()
        {
            try
            {
                var contact = await Contacts.PickContactAsync();
                if (contact == null)
                {
                    return;
                }
                else
                {
                    string s = string.Empty;
                }
            }
            catch (Exception ex) {
                string e = ex.ToString();
            }
        }
    }
}