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
using GalaSoft.MvvmLight;
using CQRSlite.Commanding;
using CQRSCode.ReadModel;
using CQRSCode.ReadModel.Dtos;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;

namespace CQRSlite.Phone.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IReadModelFacade _readmodel;
        private readonly Uri _addPageUri = new Uri("/Views/AddPage.xaml", UriKind.Relative);

        #region InventoryItems (INotifyPropertyChanged)

        private ObservableCollection<InventoryItemListDtoViewModel> _inventoryItems = new ObservableCollection<InventoryItemListDtoViewModel>();

        public ObservableCollection<InventoryItemListDtoViewModel> InventoryItems
        {
            get { return _inventoryItems; }
            set
            {
                if (_inventoryItems != value)
                {
                    _inventoryItems.Clear();
                    foreach (var item in value)
                    {
                        _inventoryItems.Add(item);
                    }
                    RaisePropertyChanged(() => this.InventoryItems);
                }
            }
        }

        #endregion

        public RelayCommand AddCommand { get; private set; }
        
        public MainPageViewModel(IReadModelFacade readmodel)
        {
            _readmodel = readmodel;

            this.AddCommand = new RelayCommand(() => Messenger.Default.Send(_addPageUri, "NavigationRequest"));

            Messenger.Default.Register<object>(this, "RefreshInventoryItems", RefreshInventoryItems);
        }

        public void RefreshInventoryItems(object obj)
        {
            this.InventoryItems = new ObservableCollection<InventoryItemListDtoViewModel>(_readmodel.GetInventoryItems().Select(s => new InventoryItemListDtoViewModel(s)));
        }

    }
}
