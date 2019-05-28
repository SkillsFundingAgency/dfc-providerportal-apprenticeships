using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings
{
    public interface ICosmosDbCollectionSettings
    {
        string StandardsCollectionId { get; }
        string FrameworkCollectionId { get; }
        string ApprenticeshipCollectionId { get; }
    }
}
