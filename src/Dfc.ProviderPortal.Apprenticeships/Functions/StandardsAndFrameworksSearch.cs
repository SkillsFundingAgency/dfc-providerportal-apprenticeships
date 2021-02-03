using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Models;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class StandardsAndFrameworksSearch
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public StandardsAndFrameworksSearch(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("StandardsAndFrameworksSearch")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            string search = req.Query["search"];
            string fromUKPRN = req.Query["UKPRN"];
            IEnumerable<IStandardsAndFrameworks> standardsAndFrameworks = null;

            if (string.IsNullOrWhiteSpace(fromUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            if (string.IsNullOrWhiteSpace(search))
            {
                return new BadRequestObjectResult($"Empty or missing search value.");
            }

            try
            {
                standardsAndFrameworks =  await _apprenticeshipService.StandardsAndFrameworksSearch(search);
                standardsAndFrameworks = _apprenticeshipService.CheckForDuplicateApprenticeships(standardsAndFrameworks, UKPRN);
                if (standardsAndFrameworks == null)
                {
                    return new NotFoundObjectResult(search);
                }

                return new OkObjectResult(standardsAndFrameworks);
            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
