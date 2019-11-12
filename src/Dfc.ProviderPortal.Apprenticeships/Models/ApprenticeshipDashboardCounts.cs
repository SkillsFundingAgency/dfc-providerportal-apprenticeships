using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Models
{
    public class ApprenticeshipDashboardCounts
    {
        public int? PublishedApprenticeshipCount { get; set; }
        public int? BulkUploadPendingCount { get; set; }
        public int? BulkUploadReadyToGoLiveCount { get; set; }
        public int? BulkUploadTotalCount { get; set; }
    }
}
