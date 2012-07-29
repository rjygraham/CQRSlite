using CQRSCode.Events;
using CQRSCode.Infrastructure;
using CQRSCode.ReadModel.Dtos;
using CQRSlite;
using System.Linq;

namespace CQRSCode.ReadModel.Handlers
{
    public class InventoryListView : IHandles<InventoryItemCreated>, IHandles<InventoryItemRenamed>, IHandles<InventoryItemDeactivated>
    {
        public void Handle(InventoryItemCreated message)
        {
            InMemoryDatabase.List.Add(new InventoryItemListDto(message.Id, message.Name));
        }

        public void Handle(InventoryItemRenamed message)
        {
            var item = InMemoryDatabase.List.Single(x => x.Id == message.Id);
            item.Name = message.NewName;
        }

        public void Handle(InventoryItemDeactivated message)
        {
            InMemoryDatabase.List = InMemoryDatabase.List.Where(w => w.Id != message.Id).ToList();
        }
    }
}