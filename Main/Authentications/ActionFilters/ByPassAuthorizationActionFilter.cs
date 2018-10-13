using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Main.Authentications.ActionFilters
{
    public class ByPassAuthorizationAttribute : Attribute, IFilterMetadata
    {
    }
}