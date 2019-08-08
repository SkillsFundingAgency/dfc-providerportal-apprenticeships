using Dfc.ProviderPortal.Apprenticeships.Interfaces.Providers;

namespace Dfc.ProviderPortal.Apprenticeships.Models.Providers
{
    public class Provideralias : IProvideralias
    {
        public object ProviderAlias { get; set; }
        public object LastUpdated { get; set; }
    }
}