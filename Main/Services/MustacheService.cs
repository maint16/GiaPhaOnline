using System;
using Main.Interfaces.Services;
using Mustache;

namespace Main.Services
{
    public class MustacheService : IMustacheService
    {
        #region Methods

        /// <inheritdoc/>
        public string Compile(string szTemplate, object data)
        {
            var formatCompiler = new FormatCompiler();
            var generator = formatCompiler.Compile(szTemplate);
            return generator.Render(data);
        }

        #endregion
    }
}