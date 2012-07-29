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
using CQRSCode.ReadModel.Dtos;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using CQRSlite.Commanding;
using CQRSCode.Commands;

namespace CQRSlite.Phone.App.ViewModels
{
    public class DetailsPageViewModel : ViewModelBase
    {

        private readonly IReadModelFacade _readModel;
        private readonly ICommandSender _commandSender;

        private InventoryItemDetailsDto _dto;

        public Guid Id
        {
            get { return _dto.Id; }
        }

        public string Name
        {
            get { return _dto.Name; }
        }

        public int CurrentCount
        {
            get { return _dto.CurrentCount; }
        }

        public DetailsPageViewModel(IReadModelFacade readModel, ICommandSender commandSender)
        {
            _readModel = readModel;
            _commandSender = commandSender;

            this.RenameCommand = new RelayCommand(() => SendNavigationRequest("RenamePage"));
            this.CheckInCommand = new RelayCommand(() => SendNavigationRequest("CheckInPage"));
            this.RemoveCommand = new RelayCommand(() => SendNavigationRequest("RemovePage"));

            this.DeactivateCommand = new RelayCommand(ExecuteDeactivate);

            Messenger.Default.Register<Guid>(this, "RefreshPage", RefreshPage);
        }

        private void RefreshPage(Guid id)
        {
            _dto = _readModel.GetInventoryItemDetails(id);
            RaisePropertyChanged(() => this.Id);
            RaisePropertyChanged(() => this.Name);
            RaisePropertyChanged(() => this.CurrentCount);
        }

        private void SendNavigationRequest(string pageName)
        {
            var uri = new Uri(String.Format("/Views/{0}.xaml?Id={1}", pageName, this.Id), UriKind.Relative);
            Messenger.Default.Send(uri, "NavigationRequest");
        }

        #region Navigation Commands (RelayCommand)

        public RelayCommand RenameCommand { get; private set; }
        public RelayCommand CheckInCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }

        #endregion

        #region Deactivate (RelayCommand)

        public RelayCommand DeactivateCommand { get; private set; }

        private void ExecuteDeactivate()
        {
            _commandSender.Send(new DeactivateInventoryItem(_dto.Id, _dto.Version));
            Messenger.Default.Send(new object(), "NavigateBack");
        }

        #endregion

    }
}
