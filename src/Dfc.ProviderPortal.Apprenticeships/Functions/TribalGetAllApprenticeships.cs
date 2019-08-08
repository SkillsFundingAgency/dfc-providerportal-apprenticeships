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
using System.Collections.Generic;
using Dfc.ProviderPortal.Apprenticeships.Models.Tribal;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class TribalGetAllApprenticeships
    {
        [FunctionName("TribalGetAllApprenticeships")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] IApprenticeshipService apprenticeshipService)
        {
            string fromQuery = req.Query["UKPRN"];
            List<Apprenticeship> apprenticeships = null;
            try
            {
                apprenticeships = (List<Apprenticeship>)await apprenticeshipService.GetApprenticeshipCollection();
                if (apprenticeships == null)
                    return new NotFoundObjectResult("Could not retrieve apprenticeships");

                var tribalProviders = (List<TribalProvider>) apprenticeshipService.ApprenticeshipsToTribalProviders(apprenticeships);
                return new OkObjectResult(tribalProviders);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
