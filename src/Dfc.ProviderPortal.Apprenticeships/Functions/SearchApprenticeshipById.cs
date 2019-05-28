using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class SearchApprenticeshipById
    {
        [FunctionName("SearchApprenticeshipById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] IApprenticeshipService apprenticeshipService)
        {
            string fromQuery = req.Query["id"];
            Apprenticeship persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
            {
                return new BadRequestObjectResult($"Empty or missing id value.");
            }

            if (!Guid.TryParse(fromQuery, out Guid id))
            {
                return new BadRequestObjectResult($"Invalid id value. Expected a non-empty valid {nameof(Guid)}");
            }

            try
            {
                persisted = (Apprenticeship)await apprenticeshipService.GetApprenticeshipById(id);

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
