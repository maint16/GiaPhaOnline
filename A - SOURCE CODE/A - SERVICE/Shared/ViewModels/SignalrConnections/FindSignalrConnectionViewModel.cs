using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.SignalrConnections
{
    public class FindSignalrConnectionViewModel
    {
        #region Properties

        /// <summary>
        /// Id of connection.
        /// </summary>
        public TextSearch Id { get; set; }

        /// <summary>
        /// Owner of connection.
        /// </summary>
        public int? Owner { get; set; }

        /// <summary>
        /// Range of created time.
        /// </summary>
        public Range<double?,double?> CreatedTime { get; set; }

        #endregion
    }
}