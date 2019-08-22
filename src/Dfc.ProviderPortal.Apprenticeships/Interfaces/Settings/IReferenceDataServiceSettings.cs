using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings
{
    public interface IReferenceDataServiceSettings
    {
        string ApiUrl { get; set; }
        string ApiKey { get; set; }
    }
}
