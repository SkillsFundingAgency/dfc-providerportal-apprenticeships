﻿using Dfc.ProviderPortal.Apprenticeships.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Models
{
    public interface IStandardsAndFrameworks
    {

        //Standard Model
        int? StandardCode { get; }
        string Version { get; }
        string StandardName { get; }
        string StandardSectorCode { get; }
        string URLLink { get; }
        string OtherBodyApprovalRequired  { get; }

        //Generic
        ApprenticeshipType ApprenticeshipType { get; }
        Guid id { get; set; } // Cosmos DB id
        DateTime EffectiveFrom { get; }
        DateTime? CreatedDateTimeUtc { get; }
        DateTime? ModifiedDateTimeUtc { get; }
        int? RecordStatusId { get; }

        //Framework Model
        int? FrameworkCode { get; }
        int? ProgType { get; }
        int? PathwayCode { get; }
        string PathwayName { get; }
        string NasTitle { get; }
        DateTime EffectiveTo { get; }
        string SectorSubjectAreaTier1 { get; }
        string SectorSubjectAreaTier2 { get; }

    }
}
