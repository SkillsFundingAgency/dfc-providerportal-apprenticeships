using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Models;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships
{
    public interface IApprenticeshipMigrationReportService
    {
        Task CreateApprenticeshipReport(ApprenticeshipMigrationReport report);
    }

}
