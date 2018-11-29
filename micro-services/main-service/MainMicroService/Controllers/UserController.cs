using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientShared.Enumerations;
using MainBusiness.Interfaces;
using MainBusiness.Interfaces.Domains;
using MainBusiness.Models.Users;
using MainDb.Interfaces;
using MainDb.Models.Entities;
using MainMicroService.Constants;
using MainMicroService.Constants.RealTime;
using MainMicroService.Interfaces.Services;
using MainModel.Models;
using MainShared.Resources;
using MainShared.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceShared.Authentications.ActionFilters;
using ServiceShared.Interfaces.Services;
using ServiceShared.Models;
using SkiaSharp;
using VgySdk.Interfaces;

namespace MainMicroService.Controllers
{
    [Route("api/user")]
    public class UserController : ApiBaseController
    {
        #region Constructors

        /// <summary>
        ///     Initiate controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="baseTimeService"></param>
        /// <param name="relationalDbService"></param>
        /// <param name="encryptionService"></param>
        /// <param name="profileService"></param>
        /// <param name="systemBaseTimeService"></param>
        /// <param name="externalAuthenticationService"></param>
        /// <param name="sendMailService"></param>
        /// <param name="emailCacheService"></param>
        /// <param name="jwtConfigurationOptions"></param>
        /// <param name="applicationSettings"></param>
        /// <param name="logger"></param>
        /// <param name="vgyService"></param>
        /// <param name="profileCacheService"></param>
        /// <param name="captchaService"></param>
        /// <param name="realTimeService"></param>
        /// <param name="userDomain"></param>
        public UserController(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IBaseTimeService baseTimeService,
            IBaseRelationalDbService relationalDbService,
            IBaseEncryptionService encryptionService,
            IAppProfileService profileService,
            IBaseTimeService systemBaseTimeService,
            IExternalAuthenticationService externalAuthenticationService,
            ISendMailService sendMailService,
            IEmailCacheService emailCacheService,
            IOptions<AppJwtModel> jwtConfigurationOptions,
            IOptions<ApplicationSetting> applicationSettings,
            ILogger<UserController> logger,
            IVgyService vgyService,
            IBaseKeyValueCacheService<int, User> profileCacheService,
            ICaptchaService captchaService,
           
            IUserDomain userDomain) : base(
            unitOfWork, mapper, baseTimeService,
            relationalDbService, profileService)
        {
            _logger = logger;
            _profileService = profileService;
            _sendMailService = sendMailService;
            _emailCacheService = emailCacheService;
            _captchaService = captchaService;
          
            _userDomain = userDomain;
        }

        #endregion

       

        #region Properties

        /// <summary>
        ///     Logging instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     Instance which is for accessing identity attached in request.
        /// </summary>
        private readonly IAppProfileService _profileService;

        /// <summary>
        ///     Send email service
        /// </summary>
        private readonly ISendMailService _sendMailService;

        /// <summary>
        ///     Email cache service.
        /// </summary>
        private readonly IEmailCacheService _emailCacheService;

        /// <summary>
        ///     Service which is for checking captcha.
        /// </summary>
        private readonly ICaptchaService _captchaService;

       

        private readonly IUserDomain _userDomain;

        #endregion
    }
}