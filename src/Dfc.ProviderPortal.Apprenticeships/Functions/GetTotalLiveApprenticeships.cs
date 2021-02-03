using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class GetTotalLiveApprenticeships
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public GetTotalLiveApprenticeships(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("GetTotalLiveApprenticeships")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            try
            {
                var allCourses = await _apprenticeshipService.GetTotalLiveApprenticeships();
                return new OkObjectResult(allCourses);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
