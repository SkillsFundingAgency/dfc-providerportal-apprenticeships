using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Models.Enums;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class ChangeApprenticeshipStatusForUKPRNSelection
    {
        [FunctionName("ChangeApprenticeshipStatusForUKPRNSelection")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req,
            ILogger log,
            [Inject] IApprenticeshipService apprenticeshipService)
        {

            var qryUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString()
                           ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.UKPRN;
            var qryCurrentStatus = req.RequestUri.ParseQueryString()["CurrentStatus"]?.ToString()
                                   ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.CurrentStatus;
            var qryStatusToBeChangedTo = req.RequestUri.ParseQueryString()["StatusToBeChangedTo"]?.ToString()
                                         ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.StatusToBeChangedTo;;

            if (string.IsNullOrWhiteSpace(qryUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(qryUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            if (string.IsNullOrWhiteSpace(qryCurrentStatus))
                return new BadRequestObjectResult($"Empty or missing CurrentStatus value.");

            if (!int.TryParse(qryCurrentStatus, out int intCurrentStatus))
                return new BadRequestObjectResult($"Invalid CurrentStatus value, expected a valid integer");

            RecordStatus CurrentStatus = (RecordStatus)intCurrentStatus;

            if (string.IsNullOrWhiteSpace(qryStatusToBeChangedTo))
                return new BadRequestObjectResult($"Empty or missing StatusToBeChangedTo value.");

            if (!int.TryParse(qryStatusToBeChangedTo, out int intStatusToBeChangedTo))
                return new BadRequestObjectResult($"Invalid StatusToBeChangedTo value, expected a valid integer");

            RecordStatus StatusToBeChangedTo = RecordStatus.Undefined;
            if (Enum.IsDefined(typeof(RecordStatus), intStatusToBeChangedTo))
            {
                StatusToBeChangedTo = (RecordStatus)Enum.ToObject(typeof(RecordStatus), intStatusToBeChangedTo);
            }
            else
            {
                return new BadRequestObjectResult($"StatusToBeChangedTo value cannot be parse into valid RecordStatus");
            }

            if (StatusToBeChangedTo.Equals(RecordStatus.Undefined))
            {
                return new BadRequestObjectResult($"StatusToBeChangedTo value is not allowed to be with  Undefined RecordStatus");
            }

            await apprenticeshipService.ChangeApprenticeshipStatusForUKPRNSelection(UKPRN, CurrentStatus, StatusToBeChangedTo);

            return new OkResult();
        }
    }
}
