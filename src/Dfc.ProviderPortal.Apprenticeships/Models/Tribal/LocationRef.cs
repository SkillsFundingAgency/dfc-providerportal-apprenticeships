using Dfc.ProviderPortal.Apprenticeships.Interfaces.Tribal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Models.Tribal
{
    public class LocationRef : ILocationRef
    {
        /// <summary>
        ///     Optional.
        /// </summary>
        public List<string> DeliveryModes { get; set; }

        /// <summary>
        ///     Required.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        ///     Optional.
        /// </summary>
        public string MarketingInfo { get; set; }

        /// <summary>
        ///     Optional.
        /// </summary>
        public int Radius { get; set; }

        /// <summary>
        ///     Optional.
        /// </summary>
        public string StandardInfoUrl { get; set; }

    }
}
