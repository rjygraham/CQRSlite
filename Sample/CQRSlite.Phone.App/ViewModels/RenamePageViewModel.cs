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
using GalaSoft.MvvmLight.Messaging;
using CQRSCode.ReadModel.Dtos;
using GalaSoft.MvvmLight.Command;
using CQRSCode.Commands;

namespace CQRSlite.Phone.App.ViewModels
{
    public class RenamePageViewModel : ViewModelBase
    {
        private readonly IReadModelFacade _readModel;
        private readonly ICommandSender _commandSender;

        private InventoryItemDetailsDto _dto;

        #region Name (INotifyPropertyChanged)

        public string Name
        {
            get { return _dto.Name; }
            set
            {
                if (_dto.Name != value)
                {
                    _dto.Name = value;
                    RaisePropertyChanged(() => this.Name);
                }
            }
        }

        #endregion

        public RenamePageViewModel(IReadModelFacade readModel, ICommandSender commandSender)
        {
            _readModel = readModel;
            _commandSender = commandSender;

            this.SaveCommand = new RelayCommand(ExecuteSave);

            Messenger.Default.Register<Guid>(this, "RefreshPage", RefreshPage);
        }

        private void RefreshPage(Guid id)
        {
            _dto = _readModel.GetInventoryItemDetails(id);
            RaisePropertyChanged(() => this.Name);
        }

        #region Save (RelayCommand)

        public RelayCommand SaveCommand { get; private set; }

        private void ExecuteSave()
        {
            _commandSender.Send(new RenameInventoryItem(_dto.Id, _dto.Name, _dto.Version));
            Messenger.Default.Send(new object(), "NavigateBack");
        }

        #endregion
    }
}
