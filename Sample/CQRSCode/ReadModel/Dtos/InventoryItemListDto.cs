using System;

namespace CQRSCode.ReadModel.Dtos
{
    public class InventoryItemListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public InventoryItemListDto(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}