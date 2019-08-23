using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Helper
{
    public class ReferenceDataServiceWrapper : IReferenceDataServiceWrapper
    {
        private readonly IReferenceDataServiceSettings _settings;
        public ReferenceDataServiceWrapper(IOptions<ReferenceDataServiceSettings> settings)
        {
            Throw.IfNull(settings, nameof(settings));
            _settings = settings.Value;
        }
        public IEnumerable<FeChoice> GetFeChoicesByUKPRN(string UKPRN)
        {
            // Call service to get data
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);
            var response = client.GetAsync($"{_settings.ApiUrl}{UKPRN}").Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                if (!json.StartsWith("["))
                    json = "[" + json + "]";

                return JsonConvert.DeserializeObject<IEnumerable<FeChoice>>(json);
            }
            return new List<FeChoice>();

        }
    }
}
