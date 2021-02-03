using System;
using System.IO;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class AddApprenticeshipMigrationReport
    {
        private readonly IApprenticeshipMigrationReportService _reportService;

        public AddApprenticeshipMigrationReport(IApprenticeshipMigrationReportService reportService)
        {
            _reportService = reportService;
        }

        [FunctionName("AddApprenticeshipMigrationReport")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req)
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
                    await _reportService.CreateApprenticeshipReport(fromBody);
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
