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
    public class GetUpdatedApprenticeshipsAsProvider
    {
        private readonly IApprenticeshipService _apprenticeshipService;

        public GetUpdatedApprenticeshipsAsProvider(IApprenticeshipService apprenticeshipService)
        {
            _apprenticeshipService = apprenticeshipService;
        }

        [FunctionName("GetUpdatedApprenticeshipsAsProvider")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            List<Apprenticeship> persisted = null;
            

            try
            {
                persisted = (List<Apprenticeship>)await _apprenticeshipService.GetUpdatedApprenticeships();
                if (persisted == null)
                    return new EmptyResult();
                var listOfProviderUKPRN = persisted.Select(x => x.ProviderUKPRN.ToString())
                                                 .Distinct()
                                                 .ToList();
                List<Apprenticeship> totalList = new List<Apprenticeship>();
                foreach (var ukprn in listOfProviderUKPRN)
                {
                    var results = _apprenticeshipService.GetApprenticeshipByUKPRN(int.Parse(ukprn)).Result;
                    if (results.Any())
                    {
                        totalList.AddRange((List<Apprenticeship>)results);
                    }
                }
                var providers = _apprenticeshipService.ApprenticeshipsToTribalProviders(totalList);
                return new OkObjectResult(providers);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
