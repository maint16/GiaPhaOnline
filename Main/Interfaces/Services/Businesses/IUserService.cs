﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Main.Models;
using Main.Models.Jwt;
using Main.ViewModels.Users;
using Shared.Models;
using SkiaSharp;

namespace Main.Interfaces.Services.Businesses
{
    public interface IUserService
    {
        #region Methods

        /// <summary>
        /// Find user login information in system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Exchange google identity with user information.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> GoogleLoginAsync(GoogleLoginViewModel model, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        /// Exchange facebook identity with user information.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> FacebookLoginAsync(FacebookLoginViewModel model, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        /// Register user information basically.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BasicRegisterResultModel> BasicRegisterAsync(RegisterAccountViewModel model, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        /// Search for users using specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<User>>> SearchUsersAsync(SearchUserViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for an user with specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> SearchUserAsync(SearchUserViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Forgot password asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ForgotPasswordResultModel> RequestPasswordResetAsync(ForgotPasswordViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Upload user profile image.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="photo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> UploadUserProfileImageAsync(int id, SKBitmap photo,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Change password asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> ChangePasswordAsync(int id, ChangePasswordViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Change user status using specific conditions.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> ChangeUserStatus(int id, ChangeUserStatusViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Generate user activation token asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GenerateUserActivationTokenResult> RequestUserActivationTokenAsync(RequestUserActivationCodeViewModel model, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Submit password reset asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SubmitPasswordResetResultModel> SubmitPasswordResetAsync(SubmitPasswordResetViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Generate access token from user information.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        JsonWebTokenViewModel GenerateUserAccessToken(User user);

        #endregion
    }
}