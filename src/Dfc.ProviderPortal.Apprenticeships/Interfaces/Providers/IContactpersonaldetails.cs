﻿namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Providers
{
    public interface IContactpersonaldetails
    {
        string[] PersonNameTitle { get; set; }
        string[] PersonGivenName { get; set; }
        string PersonFamilyName { get; set; }
        object PersonNameSuffix { get; set; }
        object PersonRequestedName { get; set; }
    }
}
