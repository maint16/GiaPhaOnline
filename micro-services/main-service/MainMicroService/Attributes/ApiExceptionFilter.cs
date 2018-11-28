using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MainMicroService.Attributes
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        #region Properties

        /// <summary>
        ///     Instance which is for logging.
        /// </summary>
        private readonly ILogger<ApiExceptionFilter> _logger;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initiate filter with dependency injection.
        /// </summary>
        /// <param name="logger"></param>
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Callback which is fired when exception occured.
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            if (context == null)
                return;

            var exception = context.Exception;
            _logger.LogError(exception, exception.Message);
        }

        #endregion
    }
}