using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
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
    public static class AddApprenticeshipMigrationReport
    {
        [FunctionName("AddApprenticeshipMigrationReport")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log,
            [Inject] IApprenticeshipMigrationReportService reportService)
        {
            using (var streamReader = new StreamReader(req.Body))
            {
                var requestBody = await streamReader.ReadToEndAsync();

                ApprenticeshipMigrationReport fromBody = null;

                try
                {
                    fromBody = JsonConvert.DeserializeObject<ApprenticeshipMigrationReport>(requestBody);
                    fromBody.Id = fromBody.Id == Guid.Empty ? Guid.NewGuid() : fromBody.Id;
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e);
                }

                try
                {
                    await reportService.CreateApprenticeshipReport(fromBody);
                }
                catch (Exception e)
                {
                    return new InternalServerErrorObjectResult(e);
                }
            }

            return new OkResult();
        }
    }
}
