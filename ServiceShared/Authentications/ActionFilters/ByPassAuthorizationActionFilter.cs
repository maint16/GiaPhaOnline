using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ServiceShared.Authentications.ActionFilters
{
    public class ByPassAuthorizationAttribute : Attribute, IFilterMetadata
    {
    }
}