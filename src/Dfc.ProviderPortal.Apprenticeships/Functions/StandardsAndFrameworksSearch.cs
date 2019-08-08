using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Apprenticeships.Models;
using System.Collections.Generic;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Models;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class StandardsAndFrameworksSearch
    {
        [FunctionName("StandardsAndFrameworksSearch")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] IApprenticeshipService apprenticeshipService)
        {
            string search = req.Query["search"];
            string fromUKPRN = req.Query["UKPRN"];
            IEnumerable<IStandardsAndFrameworks> standardsAndFrameworks = null;

            if (string.IsNullOrWhiteSpace(fromUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            if (string.IsNullOrWhiteSpace(search))
            {
                return new BadRequestObjectResult($"Empty or missing search value.");
            }

            try
            {
                standardsAndFrameworks =  await apprenticeshipService.StandardsAndFrameworksSearch(search);
                standardsAndFrameworks = apprenticeshipService.CheckForDuplicateApprenticeships(standardsAndFrameworks, UKPRN);
                if (standardsAndFrameworks == null)
                {
                    return new NotFoundObjectResult(search);
                }

                return new OkObjectResult(standardsAndFrameworks);
            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
