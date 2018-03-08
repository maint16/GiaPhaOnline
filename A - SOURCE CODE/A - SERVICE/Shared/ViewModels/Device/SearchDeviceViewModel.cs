using System;
using System.Collections.Generic;
using System.Text;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.Device
{
    public class SearchDeviceViewModel
    {
        /// <summary>
        ///     Id of Device.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        ///     Id of owner.
        /// </summary>
        public int? OwnerId { get; set; }

        /// <summary>
        ///     When the device was created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        ///     Which property & direction should be used for sorting categories.
        /// </summary>
        public Sort<DeviceSort> Sort { get; set; }

        /// <summary>
        ///     Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}
