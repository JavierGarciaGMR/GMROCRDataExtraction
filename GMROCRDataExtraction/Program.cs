using GMROCRDataExtraction.Utils;
using Google.Cloud.BigQuery.V2;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using GMROCRDataExtraction.Entities;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

ILogger _logger = loggerFactory.CreateLogger<Program>();

Credentials cr = new Credentials();

HttpClient _client = new HttpClient();

string bearerToken = "";

try
{
    GMROCRDataExtraction.Entities.Authorization bearerTokenCredentials = new GMROCRDataExtraction.Entities.Authorization()
    {
        VENDORKEY = cr.vendorKey,
        VENDORNAME = cr.vendorName,
    };

    string jsonResponseToken = JsonConvert.SerializeObject(bearerTokenCredentials, Formatting.Indented);

    var contentToken = new StringContent(jsonResponseToken, Encoding.UTF8, "application/json");

    var responseToken = await _client.PostAsync("https://localhost:44302/api/v5/Vendors/GetAccessToken", contentToken);

    var responseStringToken = await responseToken.Content.ReadAsStringAsync();

    JObject tokenResponse = JsonConvert.DeserializeObject<JObject>(responseStringToken);

    bearerToken = (string)tokenResponse["Result"]["access_token"];
}
catch (Exception ex)
{
    _logger.LogError(ex.Message);
}

//Console.WriteLine(bearerToken);

try
{
    _logger.LogInformation("Connecting with Bigquery service...");

    GoogleCredential credential = GoogleCredential.FromFile(cr.bigqueryAccessPath);

    var client = BigQueryClient.Create(cr.projectId, credential);

    string query = "SELECT * FROM `depart-team-1-prod-svc-a19m.dataset_qh_entity_extraction.gmr_hitl`";

    var result = client.ExecuteQuery(query, parameters: null);

    var rows = new List<Dictionary<string, object>>();

    foreach (var row in result)
    {
        var dict = new Dictionary<string, object>();
        foreach (var field in row.Schema.Fields)
        {
            dict[field.Name] = row[field.Name];
        }
        rows.Add(dict);
    }

    //string json = JsonConvert.SerializeObject(rows[0].Values, Formatting.Indented);

    foreach (var data in rows)
    {
        if (data.TryGetValue("details", out object valor))
        {

            JObject[] obj = JsonConvert.DeserializeObject<JObject[]>((string)valor);

            JObject firstData = obj[0];
            JObject secondData = obj[1];

            string isAMCNPortal = "T";
            string mailAddressStreet = (string)firstData["value"]["17"];
            string city = (string)firstData["value"]["14"];
            string postalCode = (string)firstData["value"]["18"];
            string state = (string)firstData["value"]["16"];
            string primaryContactNumber = (string)firstData["value"]["0"];
            string email = (string)firstData["value"]["4"];
            string primaryFirstName = (string)firstData["value"]["2"];
            string primaryLastName = (string)firstData["value"]["1"];

            string primaryDateOfBirth = (string)firstData["value"]["22"];
            if (primaryDateOfBirth != "")
            {
                DateTime date = DateTime.ParseExact(primaryDateOfBirth, "yyyyMMdd", CultureInfo.InvariantCulture);
                primaryDateOfBirth = date.ToString("MM/dd/yyyy");
            }
            
            string planCodeId = (string)firstData["value"]["20"];
            string getCodeId = (string)firstData["value"]["19"];
            string trackCodeId = (string)firstData["value"]["21"];

            //Console.WriteLine($"{postalCode} {contactNumber} {primaryFirstName} {primaryLastName} {planCodeId}");

            Account signUpRequestBody = new Account()
            {
                IsAMCNPortal = "T",
                address = new Address()
                {
                    HomeAddress = new StandarAddress()
                    {
                        Line1 = mailAddressStreet,
                        Line2 = null,
                        City = city,
                        PostalCode = postalCode,
                        StateProvinceAbbrevation = state,
                        CountryCode = "US"
                    },
                    MailingAddress = new StandarAddress()
                    {
                        Line1 = mailAddressStreet,
                        Line2 = null,
                        City = city,
                        PostalCode = postalCode,
                        StateProvinceAbbrevation = state,
                        CountryCode = "US"
                    },
                    BillingAddress = new StandarAddress()
                    {
                        Line1 = mailAddressStreet,
                        Line2 = null,
                        City = city,
                        PostalCode = postalCode,
                        StateProvinceAbbrevation = state,
                        CountryCode = "US"
                    },
                },
                contactInfo = new ContactInfo()
                {
                    PrimaryContactNumber = primaryContactNumber,
                    Email = email
                },
                membership = new MembershipBundled()
                {
                    EffectiveDate = "09/25/2024",
                    planInfo = new List<PlanInfo>()
                    {
                        new PlanInfo()
                        {
                            OwnerDescription = "0",
                            Term = "",
                            PlanName = "AirEvac Benefit",
                            PlanAmount = 0,
                            CouponCode = "6276",
                            IsActive = false,
                            IsExpired = false
                        }
                    },
                    Members = new List<Member>()
                    {
                        new Member()
                        {
                            MemberId = 0,
                            DateOfBirth = primaryDateOfBirth,
                            FirstName = primaryFirstName,
                            MiddleName = "",
                            LastName = primaryLastName,
                            Gender = "Male",
                            ParticipantType = "Primary"
                        }
                    } 
                },
                slxConstantInfo = new List<SLXConstants>()
                {
                    new SLXConstants()
                    {
                        PlanCodeID = planCodeId,
                        GetCodeId = getCodeId,
                        TrackCodeId = trackCodeId,
                        OwnerDescription = "AirEvac"
                    }
                }
            };

            string jsonString = JsonConvert.SerializeObject(signUpRequestBody, Formatting.Indented);

            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            //Console.WriteLine(jsonString);


            var response = await _client.PostAsync("https://localhost:44302/api/V6/Members/SignUpMemberBasic", content);

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("OK");
                Console.WriteLine($"{responseString}");
            }
            else
            {
                Console.WriteLine("Error en la petición");
            }

        }
    }

}
catch (Exception ex)
{
    throw new Exception(ex.Message);
}

