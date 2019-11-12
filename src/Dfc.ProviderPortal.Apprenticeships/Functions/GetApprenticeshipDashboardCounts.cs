using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class GetApprenticeshipDashboardCounts
    {
        [FunctionName("GetApprenticeshipDashboardCounts")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] IApprenticeshipService apprenticeshipService)
        {
            string fromQuery = req.Query["UKPRN"];

            if (string.IsNullOrWhiteSpace(fromQuery))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                var results = await apprenticeshipService.GetApprenticeshipDashboardCounts(UKPRN);
                if (results == null)
                    return new NotFoundObjectResult(UKPRN);

                return new OkObjectResult(results);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
