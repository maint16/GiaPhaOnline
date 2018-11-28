using MainMicroService.Interfaces.Services;
using Mustache;

namespace MainMicroService.Services
{
    public class MustacheService : IMustacheService
    {
        #region Methods

        /// <inheritdoc />
        public string Compile(string szTemplate, object data)
        {
            var formatCompiler = new FormatCompiler();
            var generator = formatCompiler.Compile(szTemplate);
            return generator.Render(data);
        }

        #endregion
    }
}