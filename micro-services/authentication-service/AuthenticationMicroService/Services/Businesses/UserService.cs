using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationDb.Interfaces;
using AuthenticationDb.Models.Entities;
using AuthenticationMicroService.Interfaces.Services;
using AuthenticationMicroService.Interfaces.Services.Businesses;
using AuthenticationMicroService.ViewModels.User;
using AuthenticationModel.Enumerations;
using AuthenticationModel.Exceptions;
using AuthenticationShared.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AuthenticationShared.Resources;

namespace AuthenticationMicroService.Services.Businesses
{
    public class UserService : IUserService
    {
        #region Properties

        private readonly IEncryptionService _encryptionService;

        private readonly IIdentityService _identityService;

        private readonly HttpContext _httpContext;

        private readonly IUnitOfWork _unitOfWork;

        //private readonly IExternalAuthenticationService _externalAuthenticationService;

        private readonly ITimeService _timeService;

        //private readonly ApplicationSetting _applicationSettings;

        private readonly IRelationalDbService _relationalDbService;

        #endregion

        #region Constructors

        public UserService(IEncryptionService encryptionService, IIdentityService identityService,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            //IExternalAuthenticationService externalAuthenticationService,
            ITimeService timeService,
            IRelationalDbService relationalDbService
            //ApplicationSetting applicationSettings
            )
        {
            _encryptionService = encryptionService;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _httpContext = httpContextAccessor.HttpContext;
            //_externalAuthenticationService = externalAuthenticationService;
            _timeService = timeService;
            //_applicationSettings = applicationSettings;
            _relationalDbService = relationalDbService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Hash the password first.
            var hashedPassword = _encryptionService.Md5Hash(model.Password);

            // Search for account which is active and information is correct.
            var users = _unitOfWork.Accounts.Search();
            users = users.Where(x =>
                x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) &&
                x.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase) && x.Type == UserKind.Basic);

            // Find the first account in database.
            var user = await users.FirstOrDefaultAsync(cancellationToken);
            if (user == null)
                throw new ApiException(HttpMessages.AccountIsNotFound, HttpStatusCode.NotFound);

            switch (user.Status)
            {
                case UserStatus.Pending:
                    throw new ApiException(HttpMessages.AccountIsPending, HttpStatusCode.Forbidden);
                case UserStatus.Disabled:
                    throw new ApiException(HttpMessages.AccountIsDisabled, HttpStatusCode.Forbidden);
            }

            return user;
        }

        #endregion
    }
}
