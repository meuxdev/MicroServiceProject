using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Dtos;
using System;
using System.Linq;
using Play.Inventory.Service.Extensions;
using Play.Inventory.Service.Clients;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly CatalogClient catalogClient;

        public ItemController(IRepository<InventoryItem> _itemsRepository, CatalogClient _catalogClient)
        {
            itemsRepository = _itemsRepository;
            catalogClient = _catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> getAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            // IEnumerable<InventoryItemDto> items = (await itemsRepository.GetAllAsync(item => item.UserId == userId))
            //             .Select(item => item.AsDto());

            var catalogItems = await catalogClient.GetCatalogItemsAsync();
            var inventoryItems = await itemsRepository.GetAllAsync(item => item.UserId == userId);

            var inventoryItemsDtos = inventoryItems.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(item => item.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemsDtos);
        }


        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await itemsRepository.GetAsync(
                item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem()
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow,
                };

                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await itemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}