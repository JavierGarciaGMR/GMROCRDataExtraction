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
using GMROCRDataExtraction.Business;
using Google.Apis.Bigquery.v2.Data;

namespace GMROCRDataExtraction.Business
{
    public class ProcessDataBusiness
    {

        private readonly ILogger _logger;
        private readonly HttpClient _client = new HttpClient();
        private readonly Credentials _credentials = new Credentials();

        public ProcessDataBusiness(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> processingDataExtracted(List<Dictionary<string, object>> rows, string bearerToken)
        {
            try
            {
                GoogleCredential credential = GoogleCredential.FromFile(_credentials.bigqueryAccessPath);

                var client = BigQueryClient.Create(_credentials.projectId, credential);

                foreach (var data in rows)
                {
                    if (data.TryGetValue("details", out object valor))
                    {
                        _logger.LogInformation("Processing file...");

                        JObject[] obj = JsonConvert.DeserializeObject<JObject[]>((string)valor);

                        JObject firstData = obj[0];
                        JObject secondData = obj[1];

                        string fileName = (string)data["file_name"];
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
                            _logger.LogInformation("OK");
                            _logger.LogInformation($"{responseString}");
                        }
                        else
                        {

                            _logger.LogError(responseString);

                            var rowsToInsert = new List<BigQueryInsertRow>
                            {
                                new BigQueryInsertRow
                                {
                                    { "file_name", fileName },
                                    { "log_message", responseString },
                                    { "created_on", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "payload", (string)valor },
                                    // Añade más columnas y valores según sea necesario
                                }
                            };

                            await client.InsertRowsAsync("dataset_qh_entity_extraction", "gmr_hitl_processor_log", rowsToInsert);
                            _logger.LogInformation("Registros insertados correctamente.");

                        }
                    }
                }
                return "All data processed";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "Error";
            }
            
        }
    }
}
