using System;
using System.Collections.Generic;
using System.Text;
using SystemDatabase.Models.Entities;

namespace Shared.ViewModels.Accounts
{
    public class SearchAccountTokenResult
    {
        public SystemDatabase.Models.Entities.Token Token { get; set; }
        public Account Account { get; set; }
    }
}
