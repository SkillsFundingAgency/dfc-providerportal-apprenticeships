using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class UpdateApprenticeship
    {
        [FunctionName("UpdateApprenticeship")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req,
            ILogger log,
            [Inject] IApprenticeshipService apprenticeshipService)
        {

            Apprenticeship apprenticeship = await req.Content.ReadAsAsync<Apprenticeship>();

            try
            {
                var updatedCourse = (Apprenticeship)await apprenticeshipService.Update(apprenticeship);
                return new OkObjectResult(updatedCourse);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
