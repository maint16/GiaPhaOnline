using System.Threading.Tasks;
using AppBusiness.Interfaces.Domains;
using AppShared.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers
{
    [Route("api/category-summary")]
    public class CategorySummaryController : Controller
    {
        #region Properties

        private readonly ICategoryDomain _categoryDomain;

        #endregion

        #region Constructor

        public CategorySummaryController(ICategoryDomain categoryDomain)
        {
            _categoryDomain = categoryDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get category summaries using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public virtual async Task<IActionResult> SearchCategorySummaries(
            [FromBody] SearchCategorySummaryViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchCategorySummaryViewModel();
                TryValidateModel(condition);
            }

            if (ModelState.IsValid)
                BadRequest(ModelState);

            var loadCategorySummariesResult = await _categoryDomain.SearchCategorySummariesAsync(condition);
            return Ok(loadCategorySummariesResult);
        }

        #endregion
    }
}