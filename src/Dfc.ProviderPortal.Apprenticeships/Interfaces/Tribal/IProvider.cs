using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Tribal
{
    public interface IProvider
    {
        int Id { get; set; }
        string Email { get; set; }
        double? EmployerSatisfaction { get; set; }
        List<IFramework> Frameworks { get; set; }
        double? LearnerSatisfaction { get; set; }
        List<ILocation> Locations { get; set; }
        string MarketingInfo { get; set; }
        string Name { get; set; }
        bool NationalProvider { get; set; }
        string Phone { get; set; }
        List<IStandard> Standards { get; set; }
        int UKPRN { get; set; }
        string Website { get; set; }
    }
}
