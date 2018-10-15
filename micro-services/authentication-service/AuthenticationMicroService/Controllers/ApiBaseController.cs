using System.Collections.Generic;
using AuthenticationDb.Interfaces;
using AuthenticationMicroService.Interfaces.Services;
using AuthenticationShared.Interfaces.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationMicroService.Controllers
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
