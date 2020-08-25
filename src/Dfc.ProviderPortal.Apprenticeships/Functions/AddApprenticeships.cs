using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class AddApprenticeships
    {
        [FunctionName("AddApprenticeships")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] IApprenticeshipService apprenticeshipService)
        {
            using (var streamReader = new StreamReader(req.Body))
            {
                var requestBody = await streamReader.ReadToEndAsync();

                // Default to processing in parallel if query parameter is not supplied
                var processInParallel = !req.Query["parallel"].ToString().Equals("false", StringComparison.OrdinalIgnoreCase);

                var apprenticeships = JsonConvert.DeserializeObject<IEnumerable<Apprenticeship>>(requestBody);

                if (processInParallel)
                {
                    await Task.WhenAll(apprenticeships.Select(a => apprenticeshipService.AddApprenticeship(a)));
                }
                else
                {
                    foreach (var app in apprenticeships)
                    {
                        await apprenticeshipService.AddApprenticeship(app);
                    }
                }

                return new OkResult();
            }
        }
    }
}
