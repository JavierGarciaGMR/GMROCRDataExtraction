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


namespace GMROCRDataExtraction.Business
{
    public class BigqueryExtractionBusiness
    {

        private readonly ILogger _logger;
        private readonly Credentials _credentials = new Credentials();
        public BigqueryExtractionBusiness(ILogger logger)
        {
            _logger = logger;
        }

        public List<Dictionary<string, object>> getBigqueryData()
        {
            var rows = new List<Dictionary<string, object>>();

            try
            {
                _logger.LogInformation("Connecting with Bigquery service to extract data...");

                GoogleCredential credential = GoogleCredential.FromFile(_credentials.bigqueryAccessPath);

                var client = BigQueryClient.Create(_credentials.projectId, credential);

                string query = $"SELECT * FROM `{_credentials.projectId}.{_credentials.dataSet}.{_credentials.dataTable}`";

                var result = client.ExecuteQuery(query, parameters: null);

                foreach (var row in result)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var field in row.Schema.Fields)
                    {
                        dict[field.Name] = row[field.Name];
                    }
                    rows.Add(dict);
                }

                return rows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return rows;
            }

        }

    }
}
