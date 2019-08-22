using Dfc.ProviderPortal.Apprenticeships.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper
{
    public interface IReferenceDataServiceWrapper
    {
        IEnumerable<FeChoice> GetFeChoicesByUKPRN(string UKPRN);
    }
}
