using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class DeleteApprenticeshipsByUKPRN
    {
        [FunctionName("DeleteApprenticeshipsByUKPRN")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req,
            ILogger log,
            [Inject] IApprenticeshipService apprenticeshipService)
        {
            log.LogInformation($"DeleteCoursesByUKPRN starting");

            string strUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString()
                              ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.UKPRN;

            List<string> messagesList = null;

            if (string.IsNullOrWhiteSpace(strUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(strUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                messagesList = await apprenticeshipService.DeleteApprenticeshipsByUKPRN(UKPRN);
                if (messagesList == null)
                    return new NotFoundObjectResult(UKPRN);

                return new OkObjectResult(messagesList);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
