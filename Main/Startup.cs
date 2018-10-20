using AppBusiness.Domain;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppBusiness.Services;
using AppDb.Interfaces;
using AppDb.Models.Contexts;
using AppDb.Models.Entities;
using AppDb.Services;
using AppModel.Models;
using AppModel.Models.ExternalAuthentication;
using AutoMapper;
using ClientShared.Enumerations;
using Main.Authentications.Handlers;
using Main.Authentications.Requirements;
using Main.Authentications.TokenValidators;
using Main.Constants;
using Main.Hubs;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.Models;
using Main.Models.Captcha;
using Main.Models.PushNotification;
using Main.Services;
using Main.Services.RealTime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Serilog;
using ServiceShared.Extensions;
using ServiceShared.Interfaces.Services;
using ServiceShared.Models;
using ServiceShared.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VgySdk.Interfaces;
using VgySdk.Service;

namespace Main
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

        /// <summary>
        ///     Callback which is fired when application starts.
        /// </summary>
        /// <param name="env"></param>
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

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services DI to app.
            AddServices(services);

            // Load jwt configuration from setting files.
            services.Configure<AppJwtModel>(Configuration.GetSection(AppConfigKeyConstant.AppJwt));
            services.Configure<ApplicationSetting>(Configuration.GetSection(nameof(ApplicationSetting)));
            services.Configure<GoogleCredential>(Configuration.GetSection(AppConfigKeyConstant.GoogleCredential));
            services.Configure<FacebookCredential>(Configuration.GetSection(AppConfigKeyConstant.FacebookCredential));
            services.Configure<FcmOption>(Configuration.GetSection(AppConfigKeyConstant.AppFirebase));
            services.Configure<SendGridSetting>(Configuration.GetSection(AppConfigKeyConstant.AppSendGrid));
            //services.Configure<PusherSetting>(Configuration.GetSection(nameof(PusherSetting)));
            services.Configure<CaptchaSetting>(Configuration.GetSection(nameof(CaptchaSetting)));

            // Build a service provider.
            var servicesProvider = services.BuildServiceProvider();
            var jwtBearerSettings = servicesProvider.GetService<IOptions<AppJwtModel>>().Value;
            var fcmOption = servicesProvider.GetService<IOptions<FcmOption>>().Value;

            // Cors configuration.
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.WithExposedHeaders("WWW-Authenticate");
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();

            // Add cors configuration to service configuration.
            services.AddCors(options => { options.AddPolicy("AllowAll", corsBuilder.Build()); });
            services.AddOptions();
            
            // This can be removed after https://github.com/aspnet/IISIntegration/issues/371
            var authenticationBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            
            authenticationBuilder.AddJwtBearer(o =>
            {
                // You also need to update /wwwroot/app/scripts/app.js
                o.SecurityTokenValidators.Clear();
                o.SecurityTokenValidators.Add(new JwtBearerValidator());

                // Initialize token validation parameters.
                var tokenValidationParameters = new TokenValidationParameters();
                tokenValidationParameters.ValidAudience = jwtBearerSettings.Audience;
                tokenValidationParameters.ValidIssuer = jwtBearerSettings.Issuer;
                tokenValidationParameters.IssuerSigningKey = jwtBearerSettings.SigningKey;

#if DEBUG
                tokenValidationParameters.ValidateLifetime = false;
#endif

                o.TokenValidationParameters = tokenValidationParameters;

                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Path.ToString()
                            .StartsWith("/HUB/", StringComparison.InvariantCultureIgnoreCase))
                            context.Token = context.Request.Query["access_token"];
                        return Task.CompletedTask;
                    }
                };
            });


            // Add automaper configuration.
            services.AddAutoMapper(options => options.AddProfile(typeof(MappingProfile)));

            // Add HttpClient factory.
            services.AddHttpClient(HttpClientGroupConstant.FcmService, x =>
            {
                x.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={fcmOption.ServerKey}");
                x.DefaultRequestHeaders.TryAddWithoutValidation("project_id", fcmOption.SenderId);
            });

            services.AddHttpClient();

            #region Signalr builder

            // Add signalr service.
            services.AddSignalR();

            // Initialize signalr policy.
            //var signalrConnectionPolicy  = 
            services.AddAuthorization(x => x.AddPolicy(PolicyConstant.DefaultSignalRPolicyName, builder =>
            {
                builder.RequireAuthenticatedUser()
                    .AddRequirements(new SolidAccountRequirement());
            }));

            services.AddAuthorization(x => x.AddPolicy(PolicyConstant.IsAdminPolicy,
                builder => { builder.AddRequirements(new RoleRequirement(new[] {UserRole.Admin})); }));

            #endregion

            #region Mvc builder

            // Construct mvc options.
            services.AddMvc(mvcOptions =>
                {
                    ////only allow authenticated users
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
#if !ALLOW_ANONYMOUS
                        .AddRequirements(new SolidAccountRequirement())
#endif
                        .Build();

                    mvcOptions.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            ;

            #endregion
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="serviceProvider"></param>
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            // Enable logging.
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            // Use in-app exception handler.
            app.UseCustomizedExceptionHandler(env);

            // Use strict transport security.
            app.UseHsts();

            // Use JWT Bearer authentication in the system.
            app.UseAuthentication();

            // Use https redirection.
            //app.UseHttpsRedirection();

            // Enable cors.
            app.UseCors("AllowAll");

            // Use signalr connection.
            app.UseSignalR(x => { x.MapHub<NotificationHub>("/hub/notification"); });

            // Enable MVC features.
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
            sqlConnection = Configuration.GetConnectionString(AppConfigKeyConstant.SqliteConnectionString);
            services.AddDbContext<RelationalDbContext>(
                options => options.UseSqlite(sqlConnection, b => b.MigrationsAssembly(nameof(Main))));
#elif USE_SQLITE_INMEMORY
            sqlConnection = Configuration.GetConnectionString(AppConfigKeyConstant.SqliteConnectionString);
            //services.AddDbContext<RelationalDbContext>(
            //    options => options.UseSqlite(sqlConnection, b => b.MigrationsAssembly(nameof(Main))));
            //services.AddScoped<DbContext, RelationalDbContext>();

            var sandboxConnection = new SqliteConnection("Data Source=:memory:");
            sandboxConnection.Open();

            using (var physicalConnection = new SqliteConnection(sqlConnection))
            {
                physicalConnection.Open();
                physicalConnection.BackupDatabase(sandboxConnection);
            }

            services.AddDbContext<RelationalDbContext>(
                options => options.UseSqlite(sandboxConnection, b => b.MigrationsAssembly(nameof(Main))));
#elif USE_AZURE_SQL
            sqlConnection = Configuration.GetConnectionString("azureSqlServerConnectionString");
            services.AddDbContext<RelationalDatabaseContext>(
                options => options.UseSqlServer(sqlConnection, b => b.MigrationsAssembly(nameof(Main))));
#elif USE_IN_MEMORY
            services.AddDbContext<InMemoryRelationalDbContext>(
                options => options.UseInMemoryDatabase("iConfess")
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

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
            services.AddDbContext<RelationalDbContext>(options => options.UseSqlServer(sqlConnection, b => b.MigrationsAssembly(nameof(Main))));
#endif

            // Add scoped.
            services.AddScoped<DbContext, RelationalDbContext>();

            // Injections configuration.
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
            services.AddScoped<IBaseRelationalDbService, BaseRelationalDbService>();

            services.AddScoped<IBaseEncryptionService, EncryptionService>();
            services.AddScoped<IAppProfileService, AppProfileService>();
            services.AddScoped<IBaseTimeService, BaseTimeService>();
            services.AddScoped<ICloudMessagingService, FcmService>();
            //            services.AddScoped<INotifyService, NotifyService>();
            services.AddScoped<ISendMailService, SendGridService>();
            services.AddScoped<IMustacheService, MustacheService>();
            services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<IPusherService, PusherService>();
            services.AddScoped<ICaptchaService, CaptchaService>();

            // Store user information in cache
            services.AddSingleton<IBaseKeyValueCacheService<int, User>, ProfileCacheService>();
            services.AddSingleton<IRealTimeConnectionCacheService, RealTimeConnectionCacheService>();

            // Initialize real-time notification service as single instance.
            services.AddSingleton<IRealTimeService, RealTimeService>();

            // Initialize vgy service.
            services.AddScoped<IVgyService, VgyService>();

            // Requirement handler.
            services.AddScoped<IAuthorizationHandler, SolidAccountRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();

            services.AddScoped<ITopicDomain, TopicDomain>();
            services.AddScoped<ICategoryDomain, CategoryDomain>();
            services.AddScoped<ICategoryGroupDomain, CategoryGroupDomain>();
            services.AddScoped<ITopicReportDomain, TopicReportDomain>();
            services.AddScoped<IReplyDomain, TopicReplyDomain>();
            services.AddScoped<IUserDomain, UserDomain>();
            services.AddScoped<IFollowTopicDomain, FollowTopicDomain>();
            services.AddScoped<IFollowCategoryDomain, FollowCategoryDomain>();
            services.AddScoped<IUserDomain, UserDomain>();
            services.AddScoped<INotificationMessageDomain, NotificationMessageDomain>();
            
            // Get email cache option.
            var emailCacheOption = (Dictionary<string, EmailCacheOption>) Configuration.GetSection("emailCache")
                .Get(typeof(Dictionary<string, EmailCacheOption>));
            var emailCacheService = new EmailCacheService();
            emailCacheService.HostingEnvironment = HostingEnvironment;
            emailCacheService.ReadConfiguration(emailCacheOption);
            services.AddSingleton<IEmailCacheService>(emailCacheService);
        }

        #endregion
    }
}