using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MainMicroService.HostedServices
{
    public class SummarizeCategoryHostedService : IHostedService
    {
        #region Constructor

        public SummarizeCategoryHostedService(IServiceProvider serviceProvider,
            ILogger<SummarizeCategoryHostedService> logger)
        {
            _timer = new Timer(SummarizeCategory, null, -1, Timeout.Infinite);
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Properties

        private readonly Timer _timer;

        private readonly ILogger _logger;

        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
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
        ///     <inheritdoc />
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual void SummarizeCategory(object state)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}