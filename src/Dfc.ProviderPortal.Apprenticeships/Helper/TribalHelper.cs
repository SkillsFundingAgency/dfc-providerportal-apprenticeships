using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Models.Regions;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Models.Enums;
using Dfc.ProviderPortal.Apprenticeships.Models.Providers;
using Dfc.ProviderPortal.Apprenticeships.Models.Tribal;
using Dfc.ProviderPortal.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Helper
{
    public class TribalHelper : ITribalHelper
    {
        private readonly IReferenceDataServiceWrapper _referenceDataServiceWrapper;
        public TribalHelper(IReferenceDataServiceWrapper referenceDataServiceWrapper)
        {
            Throw.IfNull(referenceDataServiceWrapper, nameof(referenceDataServiceWrapper));
            _referenceDataServiceWrapper = referenceDataServiceWrapper;
        }
        public TribalProvider CreateTribalProviderFromProvider(Provider provider)
        {
            var contactDetails = provider.ProviderContact.FirstOrDefault();
            var feChoice = _referenceDataServiceWrapper.GetFeChoicesByUKPRN(provider.UnitedKingdomProviderReferenceNumber).FirstOrDefault();            

            return new TribalProvider
            {
                Id = provider.ProviderId ??  int.Parse(provider.UnitedKingdomProviderReferenceNumber),
                Email = contactDetails?.ContactEmail ?? string.Empty,
                EmployerSatisfaction = feChoice?.EmployerSatisfaction ?? 0.0,
                LearnerSatisfaction = feChoice?.LearnerSatisfaction ?? 0.0,
                MarketingInfo = provider.MarketingInformation ?? string.Empty,
                Name = provider.ProviderName ?? string.Empty,
                NationalProvider = provider.NationalApprenticeshipProvider,
                UKPRN = int.Parse(provider.UnitedKingdomProviderReferenceNumber),
                Website = contactDetails?.ContactWebsiteAddress ?? string.Empty
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
                            ID = location.TribalId ?? location.LocationId,
                            Address = location.Address ?? null,
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
                if (AllLiveApprenticeshipLocations(apprenticeship.ApprenticeshipLocations))
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
                        Locations = CreateLocationRef(apprenticeship.ApprenticeshipLocations)
                    });
                }
            }
            return standards;
        }
        public List<Framework> ApprenticeshipsToFrameworks(IEnumerable<Apprenticeship> apprenticeships)
        {
            List<Framework> frameworks = new List<Framework>();

            foreach (var apprenticeship in apprenticeships)
            {
                if (AllLiveApprenticeshipLocations(apprenticeship.ApprenticeshipLocations))
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
                        Locations = CreateLocationRef(apprenticeship.ApprenticeshipLocations)
                    });
                }

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
                    ID = region.ApiLocationId,
                    Name = region.SubRegionName,
                    Address = new Address
                    {
                        Address1 = region.SubRegionName,
                        Latitude = region.Latitude,
                        Longitude = region.Longitude
                        
                    },
                    

                };
                if(!apprenticeshipLocations.Contains(location))
                apprenticeshipLocations.Add(location);
            }
            return apprenticeshipLocations;
        }
        internal List<LocationRef> CreateLocationRef(IEnumerable<ApprenticeshipLocation> locations)
        {
            List<LocationRef> locationRefs = new List<LocationRef>();
            var subRegionItemModels = new SelectRegionModel().RegionItems.SelectMany(x => x.SubRegion);
            foreach(var location in locations)
            {
                if(location.Regions != null)
                {
                    foreach(var region in location.Regions)
                    {
                        locationRefs.Add(new LocationRef
                        {
                            ID = subRegionItemModels.Where(x => x.Id == region).Select(y => y.ApiLocationId.Value).FirstOrDefault(),
                            DeliveryModes = ConvertToApprenticeshipDeliveryModes(location.DeliveryModes),
                            Radius = 10
                        });
                    }
                }
                else
                {
                    locationRefs.Add(new LocationRef
                    {
                        ID = location.TribalId ?? location.LocationId,
                        DeliveryModes = ConvertToApprenticeshipDeliveryModes(location.DeliveryModes),
                        Radius = location.Radius.HasValue ? location.Radius.Value : 0
                    });
                }

            }
            return locationRefs;

        }
        internal List<int> ConvertToApprenticeshipDeliveryModes(List<int> courseDirectoryModes)
        {
            List<int> tribalList = new List<int>();
            foreach (var mode in courseDirectoryModes)
            {
                switch(mode)
                {
                    case (int)ApprenticeShipDeliveryLocation.DayRelease:
                        {
                            tribalList.Add((int)TribalDeliveryModes.DayRelease);
                            break;
                        }
                    case (int)ApprenticeShipDeliveryLocation.BlockRelease:
                        {
                            tribalList.Add((int)TribalDeliveryModes.BlockRelease);
                            break;
                        }
                    case (int)ApprenticeShipDeliveryLocation.EmployerAddress:
                        {
                            tribalList.Add((int)TribalDeliveryModes.EmployerBased);
                            break;
                        }

                }
            }
            if(courseDirectoryModes.Count == 0)
            {
                tribalList.Add((int)TribalDeliveryModes.EmployerBased);
            }
            tribalList.Sort();
            return tribalList;
        }
        internal bool AllLiveApprenticeshipLocations(IEnumerable<ApprenticeshipLocation> locations)
        {
            if (locations.Any(x => x.RecordStatus != RecordStatus.Live))
                return false;
            else
                return true;
        }
    }
}
