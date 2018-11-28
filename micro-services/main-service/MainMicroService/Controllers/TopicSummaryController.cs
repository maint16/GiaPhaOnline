using System.Threading;
using System.Threading.Tasks;
using MainBusiness.Interfaces.Domains;
using MainShared.ViewModels.Topic;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Authentications.ActionFilters;

namespace MainMicroService.Controllers
{
    [Route("api/topic-summary")]
    public class TopicSummaryController : Controller
    {
        #region Properties

        private readonly ITopicDomain _topicDomain;

        #endregion

        #region Constructor

        public TopicSummaryController(ITopicDomain topicDomain)
        {
            _topicDomain = topicDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Search for topic summaries using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [ByPassAuthorization]
        public virtual async Task<IActionResult> SearchTopicSummaries([FromBody] SearchTopicSummaryViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchTopicSummaryViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loadTopicSummariesResult = await _topicDomain.SearchTopicSummaries(condition, CancellationToken.None);
            return Ok(loadTopicSummariesResult);
        }

        #endregion
    }
}