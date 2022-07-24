using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Dtos;
using System;
using System.Linq;
using Play.Inventory.Service.Extensions;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemsRepository;

        public ItemController(IRepository<InventoryItem> _itemsRepository)
        {
            itemsRepository = _itemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> getAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            IEnumerable<InventoryItemDto> items = (await itemsRepository.GetAllAsync(item => item.UserId == userId))
                        .Select(item => item.AsDto());
            return Ok(items);
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