using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Apprenticeships.Models;
using System.Collections.Generic;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class GetStandardByCode
    {
        [FunctionName("GetStandardByCode")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] IApprenticeshipService apprenticeshipService)
        {
            string codeFromQuery = req.Query["StandardCode"];
            string versionFromQuery = req.Query["Version"];
            List<StandardsAndFrameworks> persisted = null;

            if (string.IsNullOrWhiteSpace(codeFromQuery))
                return new BadRequestObjectResult($"Empty or missing Standard Code value.");

            if (!int.TryParse(codeFromQuery, out int standardCode))
                return new BadRequestObjectResult($"Invalid Standard Code value, expected a valid integer");

            if (string.IsNullOrWhiteSpace(versionFromQuery))
                return new BadRequestObjectResult($"Empty or missing Version value.");

            if (!int.TryParse(versionFromQuery, out int standardVersion))
                return new BadRequestObjectResult($"Invalid Version value, expected a valid integer");

            try
            {
                persisted = await apprenticeshipService.GetStandardByCode(standardCode, standardVersion);
                if (persisted == null)
                    return new NotFoundObjectResult(standardCode);

                return new OkObjectResult(persisted);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
