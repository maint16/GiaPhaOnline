using SystemDatabase.Models.Entities;
using AutoMapper;
using Shared.ViewModels.Categories;
using Shared.ViewModels.Posts;

namespace Main.Models
{
    public class MappingProfile : Profile
    {
        #region Constructor

        /// <summary>
        /// Initialize automapper mapping profile.
        /// </summary>
        public MappingProfile()
        {
            // Post mapping.
            CreateMap<AddPostViewModel, Post>();
        }

        #endregion
    }
}