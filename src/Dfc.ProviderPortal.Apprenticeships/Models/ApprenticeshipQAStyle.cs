﻿using System.Collections.Generic;

namespace Dfc.ProviderPortal.Apprenticeships.Models
{
    public class ApprenticeshipQaStyle
    {
        public int ApprenticeshipQaStyleId { get; set; }

        public int ApprenticeshipId { get; set; }

        public string CreatedByUserEmail { get; set; }

        public string CreatedDateTimeUtc { get; set; }

        public string TextQAd { get; set; }
        public bool Passed { get; set; }
        public string DetailsOfQa { get; set; }
        public List<string> FailureReasons { get; set; }
    }
}
