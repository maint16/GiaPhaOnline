using System;
using System.Linq;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AutoMapper;
using Main.Interfaces.Services;
using Main.ViewModels.Topic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class TopicController : ApiBaseController
    {
        #region Constructors

        public TopicController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IIdentityService identityService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _unitOfWork = unitOfWork;
            _databaseFunction = relationalDbService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Provide access to generic database functions.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        #endregion

        #region Methods

        /// <summary>
        ///     Add topic to system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddTopic([FromBody] AddTopicViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddTopicViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find topic

            // Find topic.
            var topics = UnitOfWork.Topics.Search();
            topics = topics.Where(x => x.CategoryId == info.CategoryId && x.Status == ItemStatus.Active);

            // Check whether topic exists or not.
            var bIsTopicAvailable = await topics.AnyAsync();
            if (!bIsTopicAvailable)
                return NotFound(new ApiResponse(HttpMessages.TopicNotFound));

            #endregion

            #region Topic initialization

            // Find identity from request.
            var identity = IdentityService.GetProfile(HttpContext);

            // Topic intialization.
            var topic = new Topic();
            topic.OwnerId = identity.Id;
            topic.CategoryId = info.CategoryId;
            topic.CategoryGroupId = info.CategoryGroupId;
            topic.Title = info.Title;
            topic.Body = info.Body;
            topic.Status = ItemStatus.Active;
            topic.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
            topic.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert topic into system.
            UnitOfWork.Topics.Insert(topic);

            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(topic);
        }

        #endregion
    }
}
