﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Tribal
{
    public interface IAddress
    {
        string Address1 { get; set; }

        string Address2 { get; set; }

        string County { get; set; }

        string Email { get; set; }

        double? Latitude { get; set; }

        double? Longitude { get; set; }

        string Phone { get; set; }

        string Postcode { get; set; }

        string Town { get; set; }

        string Website { get; set; }


    }
}
