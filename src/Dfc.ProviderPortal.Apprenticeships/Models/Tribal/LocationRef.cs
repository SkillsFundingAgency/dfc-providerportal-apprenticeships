using Dfc.ProviderPortal.Apprenticeships.Interfaces.Tribal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Models.Tribal
{
    public class LocationRef : ILocationRef
    {
        public List<int> DeliveryModes { get; set; }
        public int? ID { get; set; }
        public Guid GuidID { get; set; }
        public string MarketingInfo { get; set; }
        public int Radius { get; set; }
        public string StandardInfoUrl { get; set; }

    }
}
