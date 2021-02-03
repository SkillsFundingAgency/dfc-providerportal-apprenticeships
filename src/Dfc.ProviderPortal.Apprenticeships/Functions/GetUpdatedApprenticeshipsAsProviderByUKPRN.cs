using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class GetUpdatedApprenticeshipsAsProviderByUKPRN
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public GetUpdatedApprenticeshipsAsProviderByUKPRN(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("GetUpdatedApprenticeshipsAsProviderByUKPRN")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            string fromQuery = req.Query["UKPRN"];
            List<Apprenticeship> persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                persisted = (List<Apprenticeship>)await _apprenticeshipService.GetUpdatedApprenticeships();
                if (persisted == null)
                    return new NotFoundObjectResult(UKPRN);

                var providers = _apprenticeshipService.ApprenticeshipsToTribalProviders(persisted.Where(x => x.ProviderUKPRN == UKPRN).ToList());
                return new OkObjectResult(providers);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
