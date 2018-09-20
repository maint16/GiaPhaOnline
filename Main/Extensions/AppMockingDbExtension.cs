using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppDb.Models;
using AppDb.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Main.Extensions
{
    public static class AppMockingDbExtension
    {
        public static void AddMockingRecords(this IServiceCollection serviceCollection, IHostingEnvironment env)
        {
            #region Read data from files

            var webRootPath = env.WebRootPath;
            var dbSeedRootPath = Path.Combine(webRootPath, "DbSeed");
            var entities = new []
                {typeof(User), typeof(CategoryGroup), typeof(Category), typeof(Topic), typeof(Reply)};

            // List of tasks that load content.
            var loadContentTasks = new Dictionary<Type, Task<string>>();

            foreach (var entity in entities)
            {
                var filePath = Path.Combine(dbSeedRootPath, $"{entity.Name}.json");
                if (!File.Exists(filePath))
                    continue;

                var loadContentTask = File.ReadAllTextAsync(filePath);
                loadContentTasks.TryAdd(entity, loadContentTask);
            }

            Task.WhenAll(loadContentTasks.Values).Wait();

            #endregion

            #region Serialize object

            var columnRecords = new Dictionary<Type, string>();
            foreach (var key in loadContentTasks.Keys)
            {
                var originalContent = loadContentTasks[key].Result;
                columnRecords.Add(key, originalContent);
            }
            var dbSeedOption = new DbSeedOption();
            dbSeedOption.Columns = columnRecords;

            #endregion

            serviceCollection.AddSingleton(dbSeedOption);
        }
    }
}