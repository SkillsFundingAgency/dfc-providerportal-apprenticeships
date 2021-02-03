using System;
using System.IO;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public class AddApprenticeship
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public AddApprenticeship(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("AddApprenticeship")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            using (var streamReader = new StreamReader(req.Body))
            {
                var requestBody = await streamReader.ReadToEndAsync();

                Apprenticeship fromBody = null;
                Apprenticeship persisted = null;

                try
                {
                    fromBody = JsonConvert.DeserializeObject<Apprenticeship>(requestBody);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e);
                }

                try
                {
                    persisted = (Apprenticeship)await _apprenticeshipService.AddApprenticeship(fromBody);
                }
                catch (Exception e)
                {
                    return new InternalServerErrorObjectResult(e);
                }

                return new CreatedResult(persisted.id.ToString(), persisted);
            }
        }
        
    }
}
