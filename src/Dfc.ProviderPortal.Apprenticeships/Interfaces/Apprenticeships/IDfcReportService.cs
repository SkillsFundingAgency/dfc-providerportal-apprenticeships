using Dfc.ProviderPortal.Apprenticeships.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships
{
    public interface IDfcReportService
    {
        Task<IEnumerable<ApprenticeshipDfcReportDocument>> GetDfcReports();
    }
}
