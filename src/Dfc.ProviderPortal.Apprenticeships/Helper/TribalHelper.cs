using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Models.Regions;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Models.Providers;
using Dfc.ProviderPortal.Apprenticeships.Models.Tribal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Helper
{
    public class TribalHelper : ITribalHelper
    {
        public TribalProvider CreateTribalProviderFromProvider(Provider provider)
        {
            var contactDetails = provider.ProviderContact.FirstOrDefault();

            return new TribalProvider
            {
                Id = int.Parse(provider.UnitedKingdomProviderReferenceNumber),
                Email = contactDetails != null ? contactDetails.ContactEmail : string.Empty,
                EmployerSatisfaction = 0.0,
                LearnerSatisfaction = 0.0,
                MarketingInfo = provider.MarketingInformation,
                Name = provider.ProviderName,
                NationalProvider = provider.NationalApprenticeshipProvider,
                UKPRN = int.Parse(provider.UnitedKingdomProviderReferenceNumber),
                Website = contactDetails != null ? contactDetails.ContactWebsiteAddress : string.Empty
            };

        }
        public List<Location> ApprenticeshipLocationsToLocations(IEnumerable<ApprenticeshipLocation> locations)
        {
            List<Location> tribalLocations = new List<Location>();
            if (locations.Any())
            {
                foreach (var location in locations)
                {
                    if (location.Regions != null)
                    {
                        tribalLocations.AddRange(RegionsToLocations(location.Regions));
                    }
                    else
                    {
                        tribalLocations.Add(new Location
                        {
                            GuidID = location.Id,
                            Address = location.Address != null ? location.Address : new Address(),
                            Email = location.Address != null ? location.Address.Email : string.Empty,
                            Name = location.Name,
                            Phone = location.Phone,
                            Website = location.Address != null ? location.Address.Website : string.Empty
                        });
                    }

                }
            }

            return tribalLocations;
        }
        public List<Standard> ApprenticeshipsToStandards(IEnumerable<Apprenticeship> apprenticeships)
        {
            List<Standard> standards = new List<Standard>();
            foreach (var apprenticeship in apprenticeships)
            {

                standards.Add(new Standard
                {
                    StandardCode = apprenticeship.StandardCode.Value,
                    MarketingInfo = apprenticeship.MarketingInformation,
                    StandardInfoUrl = apprenticeship.Url,
                    Contact = new Contact
                    {
                        ContactUsUrl = apprenticeship.Url,
                        Email = apprenticeship.ContactEmail,
                        Phone = apprenticeship.ContactTelephone
                    },
                    Locations = CreateLocationRef(apprenticeship.ApprenticeshipLocations, apprenticeship)
                });
            }
            return standards;
        }
        public List<Framework> ApprenticeshipsToFrameworks(IEnumerable<Apprenticeship> apprenticeships)
        {
            List<Framework> frameworks = new List<Framework>();

            foreach (var apprenticeship in apprenticeships)
            {
                frameworks.Add(new Framework
                {
                    FrameworkCode = apprenticeship.FrameworkCode.Value,
                    FrameworkInfoUrl = apprenticeship.Url,
                    Level = !string.IsNullOrEmpty(apprenticeship.NotionalNVQLevelv2) ? int.Parse(apprenticeship.NotionalNVQLevelv2) : (int?)null,
                    //Locations
                    MarketingInfo = apprenticeship.MarketingInformation,
                    PathwayCode = apprenticeship.PathwayCode.HasValue ? apprenticeship.PathwayCode.Value : (int?)null,
                    ProgType = apprenticeship.ProgType.HasValue ? apprenticeship.ProgType.Value : (int?)null,
                    Contact = new Contact
                    {
                        ContactUsUrl = apprenticeship.ContactWebsite,
                        Email = apprenticeship.ContactEmail,
                        Phone = apprenticeship.ContactTelephone
                    },
                    Locations = CreateLocationRef(apprenticeship.ApprenticeshipLocations, apprenticeship)
                });
            }
            return frameworks;
        }
        public List<Location> RegionsToLocations(string[] regionCodes)
        {
            List<Location> apprenticeshipLocations = new List<Location>();
            var regions = new SelectRegionModel().RegionItems.SelectMany(x => x.SubRegion.Where(y => regionCodes.Contains(y.Id)));
            foreach (var region in regions)
            {
                Location location = new Location
                {
                    Name = region.SubRegionName,
                    Address = new Address
                    {
                        Latitude = region.Latitude,
                        Longitude = region.Longitude
                    },

                };
                apprenticeshipLocations.Add(location);
            }
            return apprenticeshipLocations;
        }
        internal List<LocationRef> CreateLocationRef(IEnumerable<ApprenticeshipLocation> locations, Apprenticeship apprenticeship)
        {
            List<LocationRef> locationRefs = new List<LocationRef>();
            foreach(var location in locations)
            {
                locationRefs.Add(new LocationRef
                {
                    GuidID = location.Id,
                    DeliveryModes = location.DeliveryModes,
                    Radius = location.Radius.HasValue ? location.Radius.Value : 0,
                    MarketingInfo = apprenticeship.MarketingInformation,
                    StandardInfoUrl = apprenticeship.Url
                });
            }
            return locationRefs;

        }
    }
}
