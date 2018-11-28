using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppModel.Models.ExternalAuthentication;
using Main.Constants;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Main.Services
{
    public class ExternalAuthenticationService : IExternalAuthenticationService
    {
        #region Constructor

        /// <summary>
        ///     Initialize service with injectors.
        /// </summary>
        /// <param name="googleCredentialConfigOptions"></param>
        /// <param name="facebookCredentialOptions"></param>
        public ExternalAuthenticationService(IOptions<GoogleCredential> googleCredentialConfigOptions,
            IOptions<FacebookCredential> facebookCredentialOptions)
        {
            _googleCredential = googleCredentialConfigOptions.Value;
            _facebookCredential = facebookCredentialOptions.Value;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Google authentication configs.
        /// </summary>
        private readonly GoogleCredential _googleCredential;

        /// <summary>
        ///     Facebook credential configs.
        /// </summary>
        private readonly FacebookCredential _facebookCredential;

        #endregion

        #region Methods

        /// <summary>
        ///     Get token information.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<GoogleTokenInfo> GetGoogleTokenInfoAsync(string code)
        {
            // Construct parameters which should be submitted to google api service.
            var formUrlEncodedParameters = new List<KeyValuePair<string, string>>();
            formUrlEncodedParameters.Add(new KeyValuePair<string, string>("code", code));
            formUrlEncodedParameters.Add(new KeyValuePair<string, string>("client_id", _googleCredential.ClientId));
            formUrlEncodedParameters.Add(
                new KeyValuePair<string, string>("client_secret", _googleCredential.ClientSecret));
            formUrlEncodedParameters.Add(
                new KeyValuePair<string, string>("redirect_uri", _googleCredential.RedirectUri));
            formUrlEncodedParameters.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));

            // Construct form url encoded.
            var formUrlEncodedContent = new FormUrlEncodedContent(formUrlEncodedParameters);

            // Initialize http client to make download request.
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(GoogleUrlConstant.GetAccessTokenUrl);

            // Get http response from google api.
            var httpResponseMessage = await httpClient.PostAsync("", formUrlEncodedContent);
            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;

            // Find http content.
            var httpContent = httpResponseMessage.Content;
            if (httpContent == null)
                return null;

            // Get content body.
            var szContent = await httpContent.ReadAsStringAsync();

            // Invalid content.
            if (string.IsNullOrWhiteSpace(szContent))
                return null;

            return JsonConvert.DeserializeObject<GoogleTokenInfo>(szContent);
        }

        /// <inheritdoc />
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<FacebookTokenInfo> GetFacebookTokenInfoAsync(string code)
        {
            // Parameters which will be submitted to facebook api service.
            var parameters = new Dictionary<string, string>();
            parameters.Add("client_id", _facebookCredential.ClientId);
            parameters.Add("client_secret", _facebookCredential.ClientSecret);
            parameters.Add("redirect_uri", _facebookCredential.RedirectUri);
            parameters.Add("code", code);

            // Initialize request to facebook service api.
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(FacebookUrlConstant.GetFacebookTokenInfoUrl);

            // Full url to call.
            var szUrl = "";

            if (parameters.Count > 0)
            {
                var szQueryString = parameters.Select(x => $"{x.Key} = {parameters[x.Key]}");
                szUrl = $"{FacebookUrlConstant.GetFacebookTokenInfoUrl}?{szQueryString}";
            }

            // Get http response from api service.
            var httpResponseMessage = await httpClient.GetAsync(szUrl);
            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
                return null;

            var httpContent = httpResponseMessage.Content;
            if (httpContent == null)
                return null;

            var szHttpContent = await httpContent.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(szHttpContent))
                return null;

            return JsonConvert.DeserializeObject<FacebookTokenInfo>(szHttpContent);
        }

        /// <summary>
        ///     Get Google basic information.
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns></returns>
        public async Task<GoogleProfile> GetGoogleBasicProfileAsync(string idToken)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{GoogleUrlConstant.GetGoogleProfileUrl}?id_token={idToken}");
            var httpResponseMessage = httpClient.GetAsync("").Result;

            // Not success.
            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;

            // Get HttpContent from response.
            var httpContent = httpResponseMessage.Content;
            if (httpContent == null)
                return null;

            var szHttpContent = await httpContent.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(szHttpContent))
                return null;

            return JsonConvert.DeserializeObject<GoogleProfile>(szHttpContent);
        }

        /// <summary>
        ///     Exchange access token returned from facebook api service for profile.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<FacebookProfile> GetFacebookBasicProfileAsync(string accessToken)
        {
            // Initialize client to make a request to api endpoint.
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(FacebookUrlConstant.GetFacebookProfileUrl);

            // Construct url.
            var szUrl = FacebookUrlConstant.GetFacebookProfileUrl;

            // Construct request parameters.
            var parameters = new Dictionary<string, string>();
            parameters.Add("fields", "id%2Cname%2Cemail");
            parameters.Add("access_token", accessToken);

            if (parameters.Count > 0)
            {
                var szQueryString = string.Join("&", parameters.Select(x => $"{x.Key}={parameters[x.Key]}"));
                szUrl = $"{szUrl}?{szQueryString}";
            }

            // Get http response.
            var httpResponseMessage = await httpClient.GetAsync(szUrl);
            if (httpResponseMessage == null)
                return null;

            // Response is not successful.
            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;

            // Content is invalid.
            var httpContent = httpResponseMessage.Content;
            if (httpContent == null)
                return null;

            // Content is invalid.
            var szHttpContent = await httpContent.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(szHttpContent))
                return null;

            return JsonConvert.DeserializeObject<FacebookProfile>(szHttpContent);
        }

        #endregion
    }
}