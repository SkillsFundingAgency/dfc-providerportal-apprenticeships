using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings
{
    public interface ICosmosDbSettings
    {
        string EndpointUri { get; }
        string PrimaryKey { get; }
        string DatabaseId { get; }
        int RecentCount { get; }
    }
}
