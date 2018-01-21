using SystemConstant.Enumerations.Order;

namespace Shared.Models
{
    public class Sort<T>
    {
        #region Properties

        /// <summary>
        /// Sort property.
        /// </summary>
        public T Property { get; set; }

        /// <summary>
        /// Direction of sorting.
        /// </summary>
        public SortDirection Direction { get; set; }

        #endregion
    }
}