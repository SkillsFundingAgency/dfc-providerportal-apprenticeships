﻿using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings;
using Dfc.ProviderPortal.Apprenticeships.Models.Providers;
using Dfc.ProviderPortal.Packages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace Dfc.ProviderPortal.Apprenticeships.Helper
{
    public class ProviderServiceWrapper : IProviderServiceWrapper
    {
        private readonly IProviderServiceSettings _settings;
        public ProviderServiceWrapper(IProviderServiceSettings settings)
        {
            Throw.IfNull(settings, nameof(settings));
            _settings = settings;
        }
        public IEnumerable<Provider> GetProviderByUKPRN(string UKPRN)
        {
            // Call service to get data
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);
            var response = client.GetAsync($"{_settings.ApiUrl}GetProviderByPRN?PRN={UKPRN}").Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                if (!json.StartsWith("["))
                    json = "[" + json + "]";

                return JsonConvert.DeserializeObject<IEnumerable<Provider>>(json);
            }
            return new List<Provider>();

        }
    }
}
