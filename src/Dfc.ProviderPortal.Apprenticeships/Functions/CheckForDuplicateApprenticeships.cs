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
using System.Collections.Generic;
using Dfc.ProviderPortal.Apprenticeships.Models;
using System.Linq;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class CheckForDuplicateApprenticeships
    {
        [FunctionName("CheckForDuplicateApprenticeships")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] IApprenticeshipService apprenticeshipService)
        {
            string fromQuery = req.Query["UKPRN"];
            string fromQueryCode = req.Query["code"];
            string type = req.Query["type"];
            List<Apprenticeship> persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            if (!int.TryParse(fromQueryCode, out int code))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");
            try
            {
                persisted = (List<Apprenticeship>)await apprenticeshipService.GetApprenticeshipByUKPRN(UKPRN);
                if (persisted == null)
                    return new NotFoundObjectResult(UKPRN);

                IEnumerable<Apprenticeship> duplicate = null;

                switch (type.ToLower())
                {
                    case "standard":
                        {
                            duplicate = persisted.Where(x => x.StandardCode.Value == code);
                            break;
                        }
                    case "framework":
                        {
                            duplicate = persisted.Where(x => x.FrameworkCode == code);
                            break;
                        }
                    default:
                        {
                            return new NotFoundResult();
                        }
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
