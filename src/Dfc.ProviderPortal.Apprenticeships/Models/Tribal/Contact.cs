using Dfc.ProviderPortal.Apprenticeships.Interfaces.Tribal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Models.Tribal
{
    public class Contact : IContact
    {
        public string ContactUsUrl { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

    }
}
