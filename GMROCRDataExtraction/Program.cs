using GMROCRDataExtraction.Business;
using Microsoft.Extensions.Logging;


using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

ILogger _logger = loggerFactory.CreateLogger<Program>();

AcessTokenBusiness _bearerAccessToken = new AcessTokenBusiness(_logger);

BigqueryExtractionBusiness _bigqueryExtractionBusiness = new BigqueryExtractionBusiness(_logger);

ProcessDataBusiness _processDataBusiness = new ProcessDataBusiness(_logger);

var rows = new List<Dictionary<string, object>>();

//Getting access token from GenericAPI
string bearerToken = await _bearerAccessToken.getBearerAccessToken();

//Connectiong with bigquery service to extract data
rows = _bigqueryExtractionBusiness.getBigqueryData();

//Prcocessing data extracted
await _processDataBusiness.processingDataExtracted(rows, bearerToken);



