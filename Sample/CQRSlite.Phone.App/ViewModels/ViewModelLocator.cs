using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CQRSlite.Phone.App.Infrastructure;
using CQRSlite.Config;

namespace CQRSlite.Phone.App.ViewModels
{
    public class ViewModelLocator
    {
        private static IServiceLocator _serviceLocator = new FunqServiceLocator();
        public static IServiceLocator ServiceLocator
        {
            get { return _serviceLocator; }
        }

        public MainPageViewModel MainPageViewModel
        {
            get { return _serviceLocator.GetService<MainPageViewModel>(); }
        }

        public AddPageViewModel AddPageViewModel
        {
            get { return _serviceLocator.GetService<AddPageViewModel>(); }
        }

        public DetailsPageViewModel DetailsPageViewModel
        {
            get { return _serviceLocator.GetService<DetailsPageViewModel>(); }
        }

        public RenamePageViewModel RenamePageViewModel
        {
            get { return _serviceLocator.GetService<RenamePageViewModel>(); }
        }

        public CheckInPageViewModel CheckInPageViewModel
        {
            get { return _serviceLocator.GetService<CheckInPageViewModel>(); }
        }

        public RemovePageViewModel RemovePageViewModel
        {
            get { return _serviceLocator.GetService<RemovePageViewModel>(); }
        }

    }
}
