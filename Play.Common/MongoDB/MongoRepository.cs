using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Play.Common.MongoDB
{

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {

        private readonly IMongoCollection<T> dbCollection;

        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName); // Get the collection
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync(); // returns all the items in the collection
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync(); // returns all the items in the collection
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id); // building filter for id
            return await dbCollection.Find(filter).FirstOrDefaultAsync(); // using the find 
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync(); // using the find 
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                // null entity validation
                throw new ArgumentNullException(nameof(entity));
            }

            // adding to the collection
            await dbCollection.InsertOneAsync(entity);
        }


        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                // null entity validation
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(e => e.Id, entity.Id); // building the filter based on the id
            await dbCollection.ReplaceOneAsync(filter, entity); // replace the item based on the filter 
        }


        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(item => item.Id, id); // filter based on the id
            await dbCollection.DeleteOneAsync(filter); // deletes one async
        }


    }
}