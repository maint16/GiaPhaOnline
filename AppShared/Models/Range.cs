namespace AppShared.Models
{
    public class Range<TFrom, TTo>
    {
        #region Properties

        /// <summary>
        ///     When the date starts.
        /// </summary>
        public TFrom From { get; set; }

        /// <summary>
        ///     When the date ends.
        /// </summary>
        public TTo To { get; set; }

        #endregion
    }
}