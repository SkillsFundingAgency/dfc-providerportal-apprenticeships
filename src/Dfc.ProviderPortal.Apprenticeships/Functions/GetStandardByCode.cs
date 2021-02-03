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
    public class GetStandardByCode
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public GetStandardByCode(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("GetStandardByCode")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
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
                persisted = await _apprenticeshipService.GetStandardByCode(standardCode, standardVersion);
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
