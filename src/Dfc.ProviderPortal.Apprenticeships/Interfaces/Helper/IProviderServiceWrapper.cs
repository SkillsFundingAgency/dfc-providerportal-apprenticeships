using Dfc.ProviderPortal.Apprenticeships.Models.Providers;
using System.Collections.Generic;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper
{
    public interface IProviderServiceWrapper
    {
        IEnumerable<Provider> GetProviderByUKPRN(string UKPRN);
    }
}
