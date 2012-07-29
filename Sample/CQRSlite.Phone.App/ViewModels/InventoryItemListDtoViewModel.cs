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
using CQRSCode.ReadModel.Dtos;

namespace CQRSlite.Phone.App.ViewModels
{
    public class InventoryItemListDtoViewModel
    {
        private readonly InventoryItemListDto _dto;

        public Guid Id
        {
            get { return _dto.Id; }
        }

        public string Name
        {
            get { return _dto.Name; }
        }

        public string DetailsNavigateUri
        {
            get { return String.Format("/Views/DetailsPage.xaml?Id={0}", _dto.Id); }
        }

        public InventoryItemListDtoViewModel(InventoryItemListDto dto)
        {
            _dto = dto;
        }

    }
}
