using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Services
{
    public interface IApprenticeshipService
    {
        Task<IEnumerable<IStandardsAndFrameworks>> StandardsAndFrameworksSearch(string search);
    }
}
