using Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Settings
{
    public class ReferenceDataServiceSettings : IReferenceDataServiceSettings
    {
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
    }
}
