using Microsoft.Extensions.Logging;
using GMROCRDataExtraction.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMROCRDataExtraction.Business
{
    public class BearerAccessToken
    {
        private readonly ILogger _logger;
        private readonly Credentials _credentials = new Credentials();
        private readonly HttpClient _client = new HttpClient();
        private readonly string _accessToken;string bearerToken = "";

        public BearerAccessToken(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> getBearerAccessToken()
        {
            _logger.LogInformation("Getting bearer access token");

            try
            {
                GMROCRDataExtraction.Entities.Authorization bearerTokenCredentials = new GMROCRDataExtraction.Entities.Authorization()
                {
                    VENDORKEY = _credentials.vendorKey,
                    VENDORNAME = _credentials.vendorName,
                };

                string jsonResponseToken = JsonConvert.SerializeObject(bearerTokenCredentials, Formatting.Indented);

                var contentToken = new StringContent(jsonResponseToken, Encoding.UTF8, "application/json");

                var responseToken = await _client.PostAsync("https://localhost:44302/api/v5/Vendors/GetAccessToken", contentToken);

                var responseStringToken = await responseToken.Content.ReadAsStringAsync();

                JObject tokenResponse = JsonConvert.DeserializeObject<JObject>(responseStringToken);

                return bearerToken = (string)tokenResponse["Result"]["access_token"];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "Error to process...";
            }
        }
        
    }
}
