using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class GetUpdatedApprenticeships
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public GetUpdatedApprenticeships(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("GetUpdatedApprenticeships")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            List<Apprenticeship> persisted = null;

            try
            {
                persisted = (List<Apprenticeship>)await _apprenticeshipService.GetUpdatedApprenticeships();
                if (persisted == null)
                    return new EmptyResult();

                return new OkObjectResult(persisted);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
