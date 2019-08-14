using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Models.Providers;
using Dfc.ProviderPortal.Apprenticeships.Models.Tribal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper
{
    public interface ITribalHelper
    {
        TribalProvider CreateTribalProviderFromProvider(Provider provider);
        List<Location> ApprenticeshipLocationsToLocations(IEnumerable<ApprenticeshipLocation> locations);
        List<Standard> ApprenticeshipsToStandards(IEnumerable<Apprenticeship> apprenticeships);
        List<Framework> ApprenticeshipsToFrameworks(IEnumerable<Apprenticeship> apprenticeships);
        List<Location> RegionsToLocations(string[] regionCodes);
    }
}
