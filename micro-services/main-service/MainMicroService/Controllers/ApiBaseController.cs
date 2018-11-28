using AppBusiness.Interfaces;
using AppDb.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Interfaces.Services;

namespace Main.Controllers
{
    public class ApiBaseController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize base controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="baseTimeService"></param>
        /// <param name="relationalDbService"></param>
        /// <param name="profileService"></param>
        public ApiBaseController(IAppUnitOfWork unitOfWork, IMapper mapper, IBaseTimeService baseTimeService,
            IBaseRelationalDbService relationalDbService, IAppProfileService profileService)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            BaseTimeService = baseTimeService;
            RelationalDbService = relationalDbService;
            ProfileService = profileService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Provides methods & repositories to access database.
        /// </summary>
        protected IAppUnitOfWork UnitOfWork;

        /// <summary>
        ///     Instance to access auto-mapper functions.
        /// </summary>
        protected IMapper Mapper;

        /// <summary>
        ///     Service which is for handling time conversion.
        /// </summary>
        protected IBaseTimeService BaseTimeService;

        /// <summary>
        ///     Service which is for using generic database function,
        /// </summary>
        protected IBaseRelationalDbService RelationalDbService;

        /// <summary>
        ///     Service which handles identity get/set.
        /// </summary>
        protected IAppProfileService ProfileService;

        #endregion
    }
}