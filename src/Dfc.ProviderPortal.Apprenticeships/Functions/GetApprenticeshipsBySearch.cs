using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Apprenticeships.Models;

namespace Dfc.ProviderPortal.Apprenticeships.Functions
{
    public static class GetApprenticeshipsBySearch
    {
        [FunctionName("GetApprenticeshipsBySearch")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string fromQuery = req.Query["search"];
            Apprenticeship apprenticeship = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
            {
                return new BadRequestObjectResult($"Empty or missing search value.");
            }

            try
            {
            //    persisted = (Course)await coursesService.GetCourseById(id);

            //    if (persisted == null)
            //    {
            //        return new NotFoundObjectResult(id);
            //    }

                return new OkObjectResult(apprenticeship);
            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
