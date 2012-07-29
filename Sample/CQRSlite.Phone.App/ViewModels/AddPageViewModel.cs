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
using GalaSoft.MvvmLight.Command;
using CQRSCode.Commands;
using GalaSoft.MvvmLight.Messaging;

namespace CQRSlite.Phone.App.ViewModels
{
    public class AddPageViewModel : ViewModelBase
    {
        private readonly ICommandSender _commandSender;

        #region Name (INotifyPropertyChanged)

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(() => this.Name);
                }
            }
        }

        #endregion

        public AddPageViewModel(ICommandSender commandSender)
        {
            _commandSender = commandSender;

            this.SaveCommand = new RelayCommand(ExecuteSave);
        }

        #region Save (RelayCommand)

        public RelayCommand SaveCommand { get; private set; }

        private void ExecuteSave()
        {
            _commandSender.Send(new CreateInventoryItem(Guid.NewGuid(), this.Name));
            this.Name = String.Empty;
            Messenger.Default.Send(new object(), "NavigateBack");
        }

        #endregion
    }
}
