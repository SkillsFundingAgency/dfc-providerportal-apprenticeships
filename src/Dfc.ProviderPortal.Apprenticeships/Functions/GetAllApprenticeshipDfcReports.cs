using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class GetAllApprenticeshipDfcReports
    {
        private readonly IDfcReportService _dfcReportService;

        public GetAllApprenticeshipDfcReports(IDfcReportService dfcReportService)
        {
            _dfcReportService = dfcReportService;
        }

        [FunctionName("GetAllDfcReports")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            try
            {
                var result = await _dfcReportService.GetDfcReports();
                return new OkObjectResult(result);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }

        }
    }
}
