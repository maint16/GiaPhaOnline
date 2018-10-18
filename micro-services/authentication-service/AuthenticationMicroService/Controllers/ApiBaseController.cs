﻿using AuthenticationMicroService.Interfaces.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationMicroService.Controllers
{
    public class ApiBaseController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize base controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="timeService"></param>
        /// <param name="relationalDbService"></param>
        /// <param name="identityService"></param>
        public ApiBaseController(IBaseUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService,
            IBaseRelationalDbService relationalDbService, IIdentityService identityService)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            TimeService = timeService;
            RelationalDbService = relationalDbService;
            IdentityService = identityService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Provides methods & repositories to access database.
        /// </summary>
        protected IBaseUnitOfWork UnitOfWork;

        /// <summary>
        ///     Instance to access auto-mapper functions.
        /// </summary>
        protected IMapper Mapper;

        /// <summary>
        ///     Service which is for handling time conversion.
        /// </summary>
        protected ITimeService TimeService;

        /// <summary>
        ///     Service which is for using generic database function,
        /// </summary>
        protected IBaseRelationalDbService RelationalDbService;

        /// <summary>
        ///     Service which handles identity get/set.
        /// </summary>
        protected IIdentityService IdentityService;

        #endregion
    }
}