﻿using System.ComponentModel;

namespace Dfc.ProviderPortal.Apprenticeships.Models.Enums
{
    public enum ApprenticeshipType
    {
        [Description("Undefined")]
        Undefined = 0,
        [Description("Standard Code")]
        StandardCode = 1,
        [Description("Framework Code")]
        FrameworkCode = 2
    }
}
