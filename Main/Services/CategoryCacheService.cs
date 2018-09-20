using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDb.Models.Entities;

namespace Main.Services
{
    public class CategoryCacheService : ValueCacheBaseService<int, Category>
    {
    }
}
