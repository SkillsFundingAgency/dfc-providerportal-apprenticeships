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
    public static class GetUpdatedApprenticeshipsAsProvider
    {
        [FunctionName("GetUpdatedApprenticeshipsAsProvider")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] IApprenticeshipService apprenticeshipService)
        {
            List<Apprenticeship> persisted = null;
            

            try
            {
                persisted = (List<Apprenticeship>)await apprenticeshipService.GetUpdatedApprenticeships();
                if (persisted == null)
                    return new EmptyResult();
                var providers = apprenticeshipService.ApprenticeshipsToTribalProviders(persisted);
                return new OkObjectResult(providers);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
