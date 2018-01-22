using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemDatabase.Interfaces;
using SystemDatabase.Models.Entities;
using AutoMapper;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Categories;
using SkiaSharp;

namespace Main.Controllers
{
    public class CategoryController : Controller
    {
        #region Properties

        /// <summary>
        /// Service which is for handling identity.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Service which is for handling time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        /// Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Instance for mapping objects.
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="identityService">Service which is for handling identity.</param>
        /// <param name="timeService">Service which is for handling time calculation.</param>
        /// <param name="unitOfWork">Instance for accessing database.</param>
        /// <param name="mapper">Instance for mapping objects</param>
        public CategoryController(IIdentityService identityService, ITimeService timeService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _identityService = identityService;
            _timeService = timeService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find a specific category by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> FindCategory([FromQuery] int id)
        {
            // Find category.
            var categories = _unitOfWork.RepositoryCategories.Search();
            categories = categories.Where(x => x.Id == id);
            
            // Find the first matched result.
            var category = await categories.FirstOrDefaultAsync();
            
            // Cannot find the category.
            if (category == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            var result = _mapper.Map<Category, CategoryViewModel>(category);
            result.Photo = Convert.ToBase64String(category.Photo);
            return Ok(category);

        }

        /// <summary>
        /// Add a category into database.
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddCategoryViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Image proccessing

            var binaryPhoto = Convert.FromBase64String(info.Photo);
            var memoryStream = new MemoryStream(binaryPhoto);
            var skManagedStream = new SKManagedStream(memoryStream);
            var skBitmap = SKBitmap.Decode(skManagedStream);
            var resizedSkBitmap = skBitmap.Resize(new SKImageInfo(512, 512), SKBitmapResizeMethod.Lanczos3);
            

            #endregion
            
            #region Category initialization

            // Find requester identity.
            var profile = _identityService.GetProfile(Request.HttpContext);

            // Initialize category.
            var category = new Category();
            category.CreatorId = profile.Id;
            category.Photo = resizedSkBitmap.Bytes;
            category.Status = CategoryStatus.Available;
            category.Name = info.Name;
            category.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Add category into database.
            _unitOfWork.RepositoryCategories.Insert(category);

            // Commit changes.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(category);
        }

        /// <summary>
        /// Edit a category by search for its index.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategory([FromQuery] int id, [FromBody] EditCategoryViewModel info)
        {
            #region Parameters validations

            if (info == null)
            {
                info = new EditCategoryViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find category

            // Find category.
            var categories = _unitOfWork.RepositoryCategories.Search();
            categories = categories.Where(x => x.Id == id);

            // Find category.
            var category = await categories.FirstOrDefaultAsync();
            if (category == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            #endregion

            #region Information update

            // Whether information has been changed or not.
            var bHasInformationChanged = false;

            // Name is specified.
            if (info.Name != null)
            {
                category.Name = info.Name;
                bHasInformationChanged = true;
            }

            // Status is specified.
            if (info.Status != null)
            {
                category.Status = info.Status.Value;
                bHasInformationChanged = true;
            }

            // Photo is defined.
            if (!string.IsNullOrEmpty(info.Photo))
            {
                var binaryPhoto = Convert.FromBase64String(info.Photo);
                var memoryStream = new MemoryStream(binaryPhoto);
                var skManagedStream = new SKManagedStream(memoryStream);
                var skBitmap = SKBitmap.Decode(skManagedStream);
                var resizedSkBitmap = skBitmap.Resize(new SKImageInfo(512, 512), SKBitmapResizeMethod.Lanczos3);

                category.Photo = resizedSkBitmap.Bytes;
                bHasInformationChanged = true;
            }

            // Commit changes to database.
            if (bHasInformationChanged)
                await _unitOfWork.CommitAsync();

            #endregion

            return Ok();
        }

        #endregion
    }
}