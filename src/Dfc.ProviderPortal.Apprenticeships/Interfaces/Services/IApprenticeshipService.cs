﻿using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Models.Enums;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Tribal;
using Dfc.ProviderPortal.Apprenticeships.Models;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Services
{
    public interface IApprenticeshipService
    {
        Task<IEnumerable<IStandardsAndFrameworks>> StandardsAndFrameworksSearch(string search);
        Task<IApprenticeship> AddApprenticeship(IApprenticeship apprenticeship);
        Task<IApprenticeship> GetApprenticeshipById(Guid id);
        Task<IStandardsAndFrameworks> GetStandardsAndFrameworksById(Guid id, int type);
        Task<IEnumerable<IApprenticeship>> GetApprenticeshipByUKPRN(int UKPRN);
        Task<IApprenticeship> Update(IApprenticeship apprenticeship);
        Task ChangeApprenticeshipStatusForUKPRNSelection(int UKPRN, RecordStatus CurrentStatus, RecordStatus StatusToBeChangedTo);
        Task<List<string>> DeleteBulkUploadApprenticeships(int UKPRN);
        Task<List<string>> DeleteApprenticeshipsByUKPRN(int UKPRN);
        Task<IEnumerable<IApprenticeship>> GetApprenticeshipCollection();
        IEnumerable<ITribalProvider> ApprenticeshipsToTribalProviders(List<Apprenticeship> apprenticeships);
        Task<IEnumerable<IApprenticeship>> GetUpdatedApprenticeships();
        IEnumerable<IStandardsAndFrameworks> CheckForDuplicateApprenticeships(IEnumerable<IStandardsAndFrameworks> standardsAndFrameworks, int UKPRN);
        Task<List<StandardsAndFrameworks>> GetStandardByCode(int standardCode, int standardVersion);
        Task<List<StandardsAndFrameworks>> GetFrameworkByCode(int frameworkCode, int progType, int pathwayCode);
        Task<ApprenticeshipDashboardCounts> GetApprenticeshipDashboardCounts(int ukprn);
        Task<int> GetTotalLiveApprenticeships();
    }
}
