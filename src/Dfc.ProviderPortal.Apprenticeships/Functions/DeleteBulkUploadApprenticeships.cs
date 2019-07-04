using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models.Enums;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class DeleteBulkUploadApprenticeships
    {
        [FunctionName("DeleteBulkUploadApprenticeships")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequestMessage req,
            ILogger log,
            [Inject] IApprenticeshipService coursesService)
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
                messagesList = await coursesService.DeleteBulkUploadApprenticeships(UKPRN);

                await coursesService.ChangeApprenticeshipStatusForUKPRNSelection(UKPRN, RecordStatus.MigrationPending,
                    RecordStatus.Archived);

                await coursesService.ChangeApprenticeshipStatusForUKPRNSelection(UKPRN,
                    RecordStatus.MigrationReadyToGoLive, RecordStatus.Archived);

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
