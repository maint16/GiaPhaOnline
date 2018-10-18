using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthenticationMicroService.Authentications.ActionFilters
{
    public class ByPassAuthorizationAttribute : Attribute, IFilterMetadata
    {
    }
}