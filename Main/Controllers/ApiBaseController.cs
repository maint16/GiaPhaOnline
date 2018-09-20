﻿using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using AutoMapper;
using Main.Interfaces.Services;
using Main.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;

namespace Main.Controllers
{
    public class ApiBaseController : Controller
    {
        #region Properties

        /// <summary>
        /// Provides methods & repositories to access database.
        /// </summary>
        protected IUnitOfWork UnitOfWork;

        /// <summary>
        /// Instance to access auto-mapper functions.
        /// </summary>
        protected IMapper Mapper;

        /// <summary>
        /// Service which is for handling time conversion.
        /// </summary>
        protected ITimeService TimeService;

        /// <summary>
        /// Service which is for using generic database function,
        /// </summary>
        protected IRelationalDbService RelationalDbService;

        /// <summary>
        /// Service which handles identity get/set.
        /// </summary>
        protected IIdentityService IdentityService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize base controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="timeService"></param>
        /// <param name="relationalDbService"></param>
        /// <param name="identityService"></param>
        public ApiBaseController(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService, IRelationalDbService relationalDbService, IIdentityService identityService)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            TimeService = timeService;
            RelationalDbService = relationalDbService;
            IdentityService = identityService;
        }

        #endregion
    }
}