using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class UpdateApprenticeship
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public UpdateApprenticeship(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("UpdateApprenticeship")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req)
        {

            Apprenticeship apprenticeship = await req.Content.ReadAsAsync<Apprenticeship>();

            try
            {
                var updatedCourse = (Apprenticeship)await _apprenticeshipService.Update(apprenticeship);
                return new OkObjectResult(updatedCourse);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
