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
       
        Task<IApprenticeship> AddApprenticeship(IApprenticeship apprenticeship);
        Task<IApprenticeship> GetApprenticeshipById(Guid id);
        Task<IEnumerable<IApprenticeship>> GetApprenticeshipByUKPRN(int UKPRN);
        Task<IApprenticeship> Update(IApprenticeship apprenticeship);
    }
}
