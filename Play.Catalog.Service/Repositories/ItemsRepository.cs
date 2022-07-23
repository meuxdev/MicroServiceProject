using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{

    public class ItemsRepository : IItemsRepository
    {
        private const string collectionName = "items";

        private readonly IMongoCollection<Item> dbCollection;

        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        public ItemsRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<Item>(collectionName); // Get the collection
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync(); // returns all the items in the collection
        }

        public async Task<Item> GetAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id); // building filter for id
            return await dbCollection.Find(filter).FirstOrDefaultAsync(); // using the find 
        }

        public async Task CreateAsync(Item entity)
        {
            if (entity == null)
            {
                // null entity validation
                throw new ArgumentNullException(nameof(entity));
            }

            // adding to the collection
            await dbCollection.InsertOneAsync(entity);
        }


        public async Task UpdateAsync(Item entity)
        {
            if (entity == null)
            {
                // null entity validation
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<Item> filter = filterBuilder.Eq(item => item.Id, entity.Id); // building the filter based on the id
            await dbCollection.ReplaceOneAsync(filter, entity); // replace the item based on the filter 
        }


        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(item => item.Id, id); // filter based on the id
            await dbCollection.DeleteOneAsync(filter); // deletes one async
        }
    }
}