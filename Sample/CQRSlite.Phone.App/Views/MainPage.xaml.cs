using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using CQRSlite.Phone.App.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace CQRSlite.Phone.App.Views
{
    public partial class MainPage : PhoneApplicationPage
    {

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            Messenger.Default.Register<Uri>(this, "NavigationRequest", (uri) => NavigationService.Navigate(uri));

            Messenger.Default.Send(new object(), "RefreshInventoryItems");

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Messenger.Default.Unregister(this);
            base.OnNavigatedFrom(e);
        }

    }
}