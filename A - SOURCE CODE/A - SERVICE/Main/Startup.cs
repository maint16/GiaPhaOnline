#define USE_SQLSERVER

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Contexts;
using SystemDatabase.Models.Entities;
using SystemDatabase.Repositories;
using SystemDatabase.Services;
using AutoMapper;
using Main.Authentications.Handlers;
using Main.Authentications.Requirements;
using Main.Authentications.TokenValidators;
using Main.Constants;
using Main.Hubs;
using Main.Interfaces;
using Main.Interfaces.Services;
using Main.Models;
using Main.Models.ExternalAuthentication;
using Main.Models.PushNotification;
using Main.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Services;
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
        /// Hosting environement configuration.
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
            services.AddDbContext<RelationalDatabaseContext>(
                options => options.UseInMemoryDatabase("iConfess"));
#else
            sqlConnection = Configuration.GetConnectionString("sqlServerConnectionString");
            services.AddDbContext<RelationalDatabaseContext>(
                options => options.UseSqlServer(sqlConnection, b => b.MigrationsAssembly(nameof(Main))));
#endif

            // Injections configuration.
            services.AddScoped<DbContext, RelationalDatabaseContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork<RelationalDatabaseContext>>();
            services.AddScoped<IDbSharedService, DbSharedService>();

            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITimeService, TimeService>();
            services.AddScoped<IPushService, FcmService>();
            services.AddScoped<INotifyService, NotifyService>();
            services.AddScoped<ISendMailService, SendGridService>();
            services.AddScoped<IMustacheService, MustacheService>();
            services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPusherService, PusherService>();

            // Store user information in cache
            services.AddSingleton<IValueCacheService<int, Account>, ProfileCacheService>();
            services.AddSingleton<IValueCacheService<int, Category>, CategoryCacheService>();
            services.AddSingleton<IRealTimeConnectionCacheService, RealTimeConnectionCacheService>();

            // Initialize real-time notification service as single instance.
            services.AddSingleton<IRealTimeNotificationService, RealTimeNotificationService>();

            // Initialize vgy service.
            services.AddScoped<IVgyService, VgyService>();

            // Requirement handler.
            services.AddScoped<IAuthorizationHandler, SolidAccountRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();

            // Get email cache option.
            var emailCacheOption = (Dictionary<string, EmailCacheOption>)Configuration.GetSection("emailCache").Get(typeof(Dictionary<string, EmailCacheOption>));
            var emailCacheService = new EmailCacheService();
            emailCacheService.HostingEnvironment = HostingEnvironment;
            emailCacheService.ReadConfiguration(emailCacheOption);
            services.AddSingleton<IEmailCacheService>(emailCacheService);

            // Load jwt configuration from setting files.
            services.Configure<JwtConfiguration>(Configuration.GetSection(nameof(JwtConfiguration)));
            services.Configure<ApplicationSetting>(Configuration.GetSection(nameof(ApplicationSetting)));
            services.Configure<GoogleCredential>(Configuration.GetSection(nameof(GoogleCredential)));
            services.Configure<FacebookCredential>(Configuration.GetSection(nameof(FacebookCredential)));
            services.Configure<FcmOption>(Configuration.GetSection(nameof(FcmOption)));
            services.Configure<SendGridSetting>(Configuration.GetSection(nameof(SendGridSetting)));
            services.Configure<PusherSetting>(Configuration.GetSection(nameof(PusherSetting)));
            
            // Build a service provider.
            var servicesProvider = services.BuildServiceProvider();
            var jwtBearerSettings = servicesProvider.GetService<IOptions<JwtConfiguration>>().Value;
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

                o.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Path.ToString().StartsWith("/HUB/", StringComparison.InvariantCultureIgnoreCase))
                            context.Token = context.Request.Query["accessToken"];
                        return Task.CompletedTask;
                    },
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

            services.AddAuthorization(x => x.AddPolicy(PolicyConstant.IsAdminPolicy, builder =>
            {
                builder.AddRequirements(new RoleRequirement(new[] {AccountRole.Admin}));
            }));

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
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1); ;

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

            // Use exception handler for errors handling.
            app.UseExceptionHandler(options =>
            {
                options.Run(
                    async context =>
                    {
                        // Mark the response status as 500.
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                        // No exception handler feature has been found.
                        if (exceptionHandlerFeature == null || exceptionHandlerFeature.Error == null)
                            return;

                        // Current environment is not development.
                        if (!env.IsDevelopment())
                            return;

                        // Initialize response asynchronously.
                        var apiResponse = new ApiResponse(exceptionHandlerFeature.Error.Message);
                        var szApiResponse = JsonConvert.SerializeObject(apiResponse);
                        await context.Response.WriteAsync(szApiResponse).ConfigureAwait(false);
                    });
            });

            // Use strict transport security.
            app.UseHsts();

            // Use JWT Bearer authentication in the system.
            app.UseAuthentication();

            // Use https redirection.
            //app.UseHttpsRedirection();

            // Enable cors.
            app.UseCors("AllowAll");

            // Enable MVC features.
            app.UseMvc();

            // Use signalr connection.
            app.UseSignalR(x => x.MapHub<NotificationHub>(RealtimeChannelConstant.NotificationHubName));
        }
        
        #endregion
    }
}