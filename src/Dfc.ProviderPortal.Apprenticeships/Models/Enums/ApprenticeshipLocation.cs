﻿using Dfc.ProviderPortal.Apprenticeships.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Models.Enums
{
    public class ApprenticeshipLocation : IApprenticeshipLocation
    {
        public Guid id { get; set; } // Cosmos DB id
        public int ApprenticeshipLocationId { get; set; }

        public Guid? LocationGuidId { get; set; }
        public int? LocationId { get; set; }
        public List<int> DeliveryModes { get; set; }

        public ApprenticeshipLocationType ApprenticeshipLocationType { get; set; }
        public LocationType LocationType { get; set; }

        public int? Radius { get; set; }

        // Standard auditing properties 
        public RecordStatus RecordStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
