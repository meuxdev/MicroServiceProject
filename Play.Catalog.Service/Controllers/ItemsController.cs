using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Controllers
{
    [ApiController] // enable -> validators errors -> automatic 400
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new() {
            new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze Sword", "Deals a small amount of dmg", 20, DateTimeOffset.UtcNow),
        };

        // GET -> /api/items
        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            return items;
        }


        // GET -> /api/items/{id} 
        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id)
        {
            var item = items.Where(item => item.Id == id).SingleOrDefault(); // single or default helps to return null by default

            if (item == null)
            {
                return NotFound();
            }
            // [ApiController] will return 204 response default
            return item;
        }

        // POST -> /api/items
        [HttpPost]
        public ActionResult<ItemDto> Create(CreateItemDto createItemDto)
        {
            var item = new ItemDto(
                Guid.NewGuid(),
                createItemDto.Name,
                createItemDto.Description,
                createItemDto.Price,
                DateTimeOffset.UtcNow);

            items.Add(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        // PUT -> /api/items/{id} 
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = items.Where(item => item.Id == id).SingleOrDefault();

            if (existingItem == null) return NotFound();

            var updatedItem = existingItem with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };

            var index = items.FindIndex(existingItem => existingItem.Id == id);
            items[index] = updatedItem;

            return NoContent(); // Put usually does not return any
        }

        // DELETE -> /api/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var index = items.FindIndex(item => item.Id == id);
            if (index < 0) // not founded the index 
            {
                return NotFound();
            }
            items.RemoveAt(index);
            return NoContent();
        }
    }
}