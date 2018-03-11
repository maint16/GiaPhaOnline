using System.Net.Http;
using System.Threading.Tasks;

namespace Main.Interfaces.Services
{
    public interface IImageUploadService
    {
        #region Methods

        /// <summary>
        /// Upload image to host asynchronously.
        /// </summary>
        /// <returns></returns>
        /// <param name="fileBytes"></param>
        /// <param name="contentType"></param>
        /// <param name="fileName"></param>
        Task<HttpResponseMessage> UploadAsync(byte[] fileBytes, string contentType, string fileName);

        #endregion
    }
}