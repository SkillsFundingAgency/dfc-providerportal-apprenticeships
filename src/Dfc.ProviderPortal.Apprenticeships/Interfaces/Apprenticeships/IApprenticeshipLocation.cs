﻿using Dfc.ProviderPortal.Apprenticeships.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships
{
    public interface IApprenticeshipLocation
    {
        Guid id { get; set; } // Cosmos DB id
        int ApprenticeshipLocationId { get; set; }

        Guid? LocationGuidId { get; set; }
        int? LocationId { get; set; }
        List<int> DeliveryModes { get; set; }

        ApprenticeshipLocationType ApprenticeshipLocationType { get; set; }

        int? Radius { get; set; }

        // Standard auditing properties 
        RecordStatus RecordStatus { get; set; }
        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        string UpdatedBy { get; set; }
    }
}
