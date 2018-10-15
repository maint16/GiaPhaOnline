using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationMicroService.Interfaces.Services;
using AuthenticationMicroService.Models.Captcha;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace AuthenticationMicroService.Services
{
    public class CaptchaService : ICaptchaService
    {
        #region Constructors

        /// <summary>
        ///     Initialize service with injectors.
        /// </summary>
        public CaptchaService(IHttpClientFactory httpClientFactory, IOptions<CaptchaSetting> captchaSetting)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _captchaSetting = captchaSetting.Value;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientAddress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> IsCaptchaValidAsync(string code, string clientAddress,
            CancellationToken cancellationToken)
        {
#if DISABLE_CAPTCHA_VALIDATION
            return true;
#else
            var queries = new List<KeyValuePair<string, string>>();
            queries.Add(new KeyValuePair<string, string>("secret", _captchaSetting.GoogleCaptchaSecret));
            queries.Add(new KeyValuePair<string, string>("response", code));

            if (!string.IsNullOrWhiteSpace(clientAddress))
                queries.Add(new KeyValuePair<string, string>("remoteip", clientAddress));

            var queryStringParamters = queries.Select(x => $"{x.Key}={x.Value}");
            var queryString = string.Join("&", queryStringParamters);
            var uri = $"{_captchaSetting.GoogleCaptchaValidationEndpoint}?{queryString}";
            var httpResponseMessage = await _httpClient.PostAsync(uri, new StringContent("{}"), cancellationToken);

            // Read the http response content.
            var httpContent = httpResponseMessage.Content;
            if (httpContent == null)
                return false;

            var content = await httpContent.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
                return false;

            var jObject = JObject.Parse(content);
            bool.TryParse(jObject["success"].ToString(), out var bIsSuccess);

            return bIsSuccess;
#endif
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Http client which is for initializing http request.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        ///     Captcha setting model.
        /// </summary>
        private readonly CaptchaSetting _captchaSetting;

        #endregion
    }
}
