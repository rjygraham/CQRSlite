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
using CQRSCode.ReadModel;
using CQRSlite.Commanding;
using CQRSCode.ReadModel.Dtos;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using CQRSCode.Commands;

namespace CQRSlite.Phone.App.ViewModels
{
    public class RemovePageViewModel : ViewModelBase
    {
        private readonly IReadModelFacade _readModel;
        private readonly ICommandSender _commandSender;

        private InventoryItemDetailsDto _dto;

        #region Count (INotifyPropertyChanged)

        private int _count;

        public int Count
        {
            get { return _count; }
            set
            {
                if (_count != value)
                {
                    _count = value;
                    RaisePropertyChanged(() => this.Count);
                }
            }
        }
        #endregion

        public RemovePageViewModel(IReadModelFacade readModel, ICommandSender commandSender)
        {
            _readModel = readModel;
            _commandSender = commandSender;

            this.SaveCommand = new RelayCommand(ExecuteSave);

            Messenger.Default.Register<Guid>(this, "RefreshPage", RefreshPage);
        }

        private void RefreshPage(Guid id)
        {
            this.Count = 0;
            _dto = _readModel.GetInventoryItemDetails(id);
        }

        #region Save (RelayCommand)

        public RelayCommand SaveCommand { get; private set; }

        private void ExecuteSave()
        {
            _commandSender.Send(new RemoveItemsFromInventory(_dto.Id, _count, _dto.Version));
            Messenger.Default.Send(new object(), "NavigateBack");
        }

        #endregion
    }
}
