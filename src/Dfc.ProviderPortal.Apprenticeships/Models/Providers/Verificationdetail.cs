using Dfc.ProviderPortal.Apprenticeships.Interfaces.Providers;

namespace Dfc.ProviderPortal.Apprenticeships.Models.Provider
{
    public class Verificationdetail : IVerificationdetail
    {
        public string VerificationAuthority { get; set; }
        public string VerificationID { get; set; }
    }
}
