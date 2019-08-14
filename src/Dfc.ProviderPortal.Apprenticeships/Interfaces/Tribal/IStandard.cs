using Dfc.ProviderPortal.Apprenticeships.Models.Tribal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Tribal
{
    public interface IStandard
    {
        IContact Contact { get; set; }

        List<LocationRef> Locations { get; set; }
        string MarketingInfo { get; set; }
        int StandardCode { get; set; }
        string StandardInfoUrl { get; set; }
    }
}
