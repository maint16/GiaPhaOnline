using AppDb.Models.Entities;
using AppShared.ViewModels.Users;
using AutoMapper;

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
            CreateMap<Topic, Topic>();
        }

        #endregion
    }
}