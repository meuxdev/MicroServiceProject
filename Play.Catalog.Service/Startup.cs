using MongoDB.Bson;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Play.Catalog.Service.Settings;
using MongoDB.Driver;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public class Startup
    {
        private ServiceSettings serviceSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Serialize configuration for support of other types. 
            // Example Guid type 
            // DateTimeOffset
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            // Configuration for the injection of the mongodb configuration. Database -> IMongoDatabase
            services.AddSingleton(serviceProvider =>
            {
                var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            // Injection of the IItems Repository
            services.AddScoped<IRepository<Item>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<Item>(database, "items");
            });

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false; //! Forcing to do not delete the async suffix during runtime 
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Catalog.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Catalog.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
