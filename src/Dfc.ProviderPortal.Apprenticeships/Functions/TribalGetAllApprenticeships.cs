using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Models.Tribal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class TribalGetAllApprenticeships
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public TribalGetAllApprenticeships(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("TribalGetAllApprenticeships")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            string fromQuery = req.Query["UKPRN"];
            List<Apprenticeship> apprenticeships = null;
            try
            {
                apprenticeships = (List<Apprenticeship>)await _apprenticeshipService.GetApprenticeshipCollection();
                if (apprenticeships == null)
                    return new NotFoundObjectResult("Could not retrieve apprenticeships");

                var tribalProviders = (List<TribalProvider>)_apprenticeshipService.ApprenticeshipsToTribalProviders(apprenticeships);
                return new OkObjectResult(tribalProviders);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
