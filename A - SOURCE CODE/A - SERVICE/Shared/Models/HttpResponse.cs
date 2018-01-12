namespace Shared.Models
{
    public class HttpResponse
    {
        #region Properties

        /// <summary>
        /// Message responded from Http service.
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initiate class with default settings.
        /// </summary>
        public HttpResponse()
        {
            
        }

        /// <summary>
        /// Initiate class with settings.
        /// </summary>
        /// <param name="message"></param>
        public HttpResponse(string message)
        {
            Message = message;
        }

        #endregion
    }
}