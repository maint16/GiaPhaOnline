namespace AuthenticationShared.Models
{
    public class ApiResponse
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
        public ApiResponse()
        {

        }

        /// <summary>
        /// Initiate class with settings.
        /// </summary>
        /// <param name="message"></param>
        public ApiResponse(string message)
        {
            Message = message;
        }

        #endregion
    }
}
