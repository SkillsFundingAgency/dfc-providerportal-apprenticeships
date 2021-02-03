using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class DeleteBulkUploadApprenticeships
    {
        private readonly IApprenticeshipService _coursesService;

        public DeleteBulkUploadApprenticeships(IApprenticeshipService coursesService)
        {
            _coursesService = coursesService;
        }

        [FunctionName("DeleteBulkUploadApprenticeships")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequestMessage req)
        {
            string strUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString()
                              ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.UKPRN;

            List<string> messagesList = null;

            if (string.IsNullOrWhiteSpace(strUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(strUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                messagesList = await _coursesService.DeleteBulkUploadApprenticeships(UKPRN);

                await _coursesService.ChangeApprenticeshipStatusForUKPRNSelection(UKPRN, RecordStatus.MigrationPending,
                    RecordStatus.Archived);

                await _coursesService.ChangeApprenticeshipStatusForUKPRNSelection(UKPRN,
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
