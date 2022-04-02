using HearMeRoar.ViewModels;
using HearMeRoar.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HearMeRoar
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Shell.SetTabBarIsVisible(this, false);
            Shell.SetNavBarIsVisible(this, false);
        }
    }


    public class NavigationFlyoutItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NavigationHeaderTemplate { get; set; }
        public DataTemplate NavigationItemTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            //Returning null, because at this point I'm not sure how to select the correct template
            return null;
        }
    }
}
