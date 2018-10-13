using AutoMapper;
using Shared.ViewModels.Users;

namespace Main.Models
{
    public class MappingProfile : Profile
    {
        #region Constructor

        /// <summary>
        ///     Initialize automapper mapping profile.
        /// </summary>
        public MappingProfile()
        {
            // Post mapping.
            //CreateMap<AddPostViewModel, Post>();
            CreateMap<LoginViewModel, LoginViewModel>();
        }

        #endregion
    }
}