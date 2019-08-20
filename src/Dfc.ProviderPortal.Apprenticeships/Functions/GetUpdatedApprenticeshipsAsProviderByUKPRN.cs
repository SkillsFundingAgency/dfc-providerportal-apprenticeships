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
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class GetUpdatedApprenticeshipsAsProviderByUKPRN
    {
        [FunctionName("GetUpdatedApprenticeshipsAsProviderByUKPRN")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] IApprenticeshipService apprenticeshipService)
        {
            string fromQuery = req.Query["UKPRN"];
            List<Apprenticeship> persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                persisted = (List<Apprenticeship>)await apprenticeshipService.GetUpdatedApprenticeships();
                if (persisted == null)
                    return new NotFoundObjectResult(UKPRN);

                var providers = apprenticeshipService.ApprenticeshipsToTribalProviders(persisted.Where(x => x.ProviderUKPRN == UKPRN).ToList());
                return new OkObjectResult(providers);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
