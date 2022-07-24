using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Extensions
{
    public static class Extensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item)
        {
            return new InventoryItemDto(item.CatalogItemId, item.Quantity, item.AcquiredDate);
        }
    }
}