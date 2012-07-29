using System;

namespace CQRSCode.ReadModel.Dtos
{
    public class InventoryItemDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CurrentCount { get; set; }
        public int Version { get; set; }

        public InventoryItemDetailsDto(Guid id, string name, int currentCount, int version)
        {
            this.Id = id;
            this.Name = name;
            this.CurrentCount = currentCount;
            this.Version = version;
        }
    }
}