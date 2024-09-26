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

string bearerToken = await _bearerAccessToken.getBearerAccessToken();

rows = _bigqueryExtractionBusiness.getBigqueryData();

await _processDataBusiness.processingDataExtracted(rows, bearerToken);



