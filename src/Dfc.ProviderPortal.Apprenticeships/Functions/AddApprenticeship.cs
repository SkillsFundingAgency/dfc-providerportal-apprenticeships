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
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class AddApprenticeship
    {
        [FunctionName("AddApprenticeship")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] IApprenticeshipService apprenticeshipService)
        {
            using (var streamReader = new StreamReader(req.Body))
            {
                var requestBody = await streamReader.ReadToEndAsync();

                Apprenticeship fromBody = null;
                Apprenticeship persisted = null;

                try
                {
                    fromBody = JsonConvert.DeserializeObject<Apprenticeship>(requestBody);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e);
                }

                try
                {
                    persisted = (Apprenticeship)await apprenticeshipService.AddApprenticeship(fromBody);
                }
                catch (Exception e)
                {
                    return new InternalServerErrorObjectResult(e);
                }

                return new CreatedResult(persisted.id.ToString(), persisted);
            }
        }
        
    }
}
