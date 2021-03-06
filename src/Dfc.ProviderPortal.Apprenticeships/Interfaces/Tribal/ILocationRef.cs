﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Tribal
{
    public interface ILocationRef
    {
        List<int> DeliveryModes { get; set; }
        int? ID { get; set; }
        int Radius { get; set; }
    }
}
