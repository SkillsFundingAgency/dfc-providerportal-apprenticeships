using Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Settings
{
    public class CosmosDbCollectionSettings : ICosmosDbCollectionSettings
    {
        public string StandardsCollectionId { get; set; }
        public string FrameworkCollectionId { get; set; }
        public string ApprenticeshipCollectionId { get; set; }
    }
}
