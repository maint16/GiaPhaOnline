using System;
using System.Collections.Generic;
using AppDb.Models.Entities;

namespace AppDb.Models
{
    public class DbSeedOption
    {
        public IDictionary<Type, string> Columns { get; set; }
    }
}