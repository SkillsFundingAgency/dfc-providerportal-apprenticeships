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
    public static class GetFrameworkByCode
    {
        [FunctionName("GetFrameworkByCode")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] IApprenticeshipService apprenticeshipService)
        {
            string codeFromQuery = req.Query["FrameworkCode"];
            string progTypeFromQuery = req.Query["ProgType"];
            string pathwayCodeFromQuery = req.Query["PathwayCode"];
            List<StandardsAndFrameworks> persisted = null;

            if (string.IsNullOrWhiteSpace(codeFromQuery))
                return new BadRequestObjectResult($"Empty or missing Framework Code value.");

            if (!int.TryParse(codeFromQuery, out int frameworkCode))
                return new BadRequestObjectResult($"Invalid Framework Code value, expected a valid integer");

            if (string.IsNullOrWhiteSpace(progTypeFromQuery))
                return new BadRequestObjectResult($"Empty or missing ProgType value.");

            if (!int.TryParse(progTypeFromQuery, out int progType))
                return new BadRequestObjectResult($"Invalid ProgType value, expected a valid integer");

            if (string.IsNullOrWhiteSpace(pathwayCodeFromQuery))
                return new BadRequestObjectResult($"Empty or missing Pathway code value.");

            if (!int.TryParse(progTypeFromQuery, out int pathwayCode))
                return new BadRequestObjectResult($"Invalid Pathway code value, expected a valid integer");

            try
            {
                persisted = await apprenticeshipService.GetFrameworkByCode(frameworkCode, progType, pathwayCode);
                if (persisted == null)
                    return new NotFoundObjectResult(frameworkCode);

                return new OkObjectResult(persisted);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
