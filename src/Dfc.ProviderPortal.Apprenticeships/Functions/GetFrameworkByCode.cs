using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class GetFrameworkByCode
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public GetFrameworkByCode(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("GetFrameworkByCode")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
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

            if (!int.TryParse(pathwayCodeFromQuery, out int pathwayCode))
                return new BadRequestObjectResult($"Invalid Pathway code value, expected a valid integer");

            try
            {
                persisted = await _apprenticeshipService.GetFrameworkByCode(frameworkCode, progType, pathwayCode);
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
