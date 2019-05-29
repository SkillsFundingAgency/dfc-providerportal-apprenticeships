using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings
{
    public interface ICosmosDbCollectionSettings
    {
        string StandardsCollectionId { get; }
        string FrameworksCollectionId { get; }
        string ApprenticeshipCollectionId { get; }
    }
}
