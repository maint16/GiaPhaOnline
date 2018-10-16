using AuthenticationBusiness.Interfaces;
using AuthenticationBusiness.Services;
using AuthenticationDb.Interfaces;
using AuthenticationDb.Interfaces.Repositories;
using AuthenticationDb.Models.Contexts;
using AuthenticationDb.Models.Entities;
using AuthenticationDb.Repositories;
using AuthenticationDb.Services;
using AuthenticationMicroService.Interfaces.Services;
using AuthenticationMicroService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationMicroService
{
    public class Startup
    {
        #region Properties

        /// <summary>
        ///     Instance stores configuration of application.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        ///     Hosting environement configuration.
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; }

        #endregion

        #region Methods

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddJsonFile($"dbSetting.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Set hosting environment.
            HostingEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add services DI to app.
            AddServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvc();
        }

        /// <summary>
        ///     Add dependency injection of services to app.
        /// </summary>
        private void AddServices(IServiceCollection services)
        {
            // Add entity framework to services collection.
            var sqlConnection = "";

#if USE_SQLITE
            sqlConnection = Configuration.GetConnectionString("sqliteConnectionString");
            services.AddDbContext<RelationalDatabaseContext>(
                options => options.UseSqlite(sqlConnection, b => b.MigrationsAssembly(nameof(Main))));
#elif USE_AZURE_SQL
            sqlConnection = Configuration.GetConnectionString("azureSqlServerConnectionString");
            services.AddDbContext<RelationalDatabaseContext>(
                options => options.UseSqlServer(sqlConnection, b => b.MigrationsAssembly(nameof(Main))));
#elif USE_IN_MEMORY
            services.AddOptions<DbSeedOption>();
            services.AddDbContext<InMemoryRelationalDbContext>(
                options => options.UseInMemoryDatabase("iConfess")
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

            services.AddMockingRecords(HostingEnvironment);
            var bDbCreated = false;
            services.AddScoped<DbContext>(context =>
            {
                var inMemoryContext = context.GetService<InMemoryRelationalDbContext>();
                if (!bDbCreated)
                {
                    inMemoryContext.Database.EnsureCreated();
                    bDbCreated = true;
                }
                return context.GetService<InMemoryRelationalDbContext>();
            });
#else
            sqlConnection = Configuration.GetConnectionString("sqlServerConnectionString");
            services.AddDbContext<RelationalDbContext>(options => options.UseSqlServer(sqlConnection, b => b.MigrationsAssembly(nameof(AuthenticationMicroService))));
            services.AddScoped<DbContext, RelationalDbContext>();
#endif

            // Injections configuration.
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRelationalDbService, RelationalDbService>();

            // Store user information in cache
            services.AddSingleton<IValueCacheService<int, User>, ProfileCacheService>();
        }

        #endregion


    }
}