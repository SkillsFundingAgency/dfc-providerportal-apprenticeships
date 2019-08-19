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
        public int Radius { get; set; }

    }
}
