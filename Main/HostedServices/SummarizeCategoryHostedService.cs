using System;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces.Domains;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Main.HostedServices
{
    public class SummarizeCategoryHostedService : IHostedService
    {
        #region Properties

        private readonly Timer _timer;

        private readonly ILogger _logger;

        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructor

        public SummarizeCategoryHostedService(IServiceProvider serviceProvider, ILogger<SummarizeCategoryHostedService> logger)
        {
            _timer = new Timer(SummarizeCategory, null, -1, Timeout.Infinite);
            _logger = logger;
            _serviceProvider = serviceProvider;
        }



        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(SummarizeCategoryHostedService)} has been started.");
            _timer.Change(0, Timeout.Infinite);

            return Task.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<ICategoryDomain>();
            }
        }

        protected virtual void SummarizeCategory(object state)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}