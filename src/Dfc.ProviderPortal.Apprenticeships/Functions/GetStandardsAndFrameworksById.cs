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
using Dfc.ProviderPortal.Apprenticeships.Models.Enums;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class GetStandardsAndFrameworkById
    {
        [FunctionName("GetStandardsAndFrameworksById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] IApprenticeshipService apprenticeshipService)
        {
            string fromQueryId = req.Query["id"];
            string fromQueryType = req.Query["type"];

            StandardsAndFrameworks persisted = null;

            if (string.IsNullOrWhiteSpace(fromQueryId))
            {
                return new BadRequestObjectResult($"Empty or missing id value.");
            }

            if (!int.TryParse(fromQueryType, out int apprenticeshipType))
                return new BadRequestObjectResult($"Invalid Type value, expected a valid integer");

            if (!Guid.TryParse(fromQueryId, out Guid id))
            {
                return new BadRequestObjectResult($"Invalid id value. Expected a non-empty valid {nameof(Guid)}");
            }

            try
            {
                persisted = (StandardsAndFrameworks)await apprenticeshipService.GetStandardsAndFrameworksById(id, apprenticeshipType);

                if (persisted == null)
                {
                    return new NotFoundObjectResult(id);
                }

                return new OkObjectResult(persisted);
            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }

    }
}
