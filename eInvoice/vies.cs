using System.Net.Http;
using System.Text;
using System.Xml;

public class ViesResult
{
    public string CountryCode   { get; set; } = default!;
    public string VatNumber     { get; set; } = default!;
    public bool   IsValid       { get; set; }
    public string? Name         { get; set; }
    public string? Address      { get; set; }
    public DateTime RequestDate { get; set; }
}

public class ViesClient
{
    private const string Endpoint  = "https://ec.europa.eu/taxation_customs/vies/services/checkVatService";
    private const string Namespace = "urn:ec.europa.eu:taxud:vies:services:checkVat:types";

    private readonly HttpClient _http;

    public ViesClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ViesResult> CheckVatAsync(string countryCode, string vatNumber)
    {
        var soapBody = BuildRequest(countryCode, vatNumber);

        using var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "");

        using var response = await _http.PostAsync(Endpoint, content);
        var xml = await response.Content.ReadAsStringAsync();

        return ParseResponse(xml);
    }

    // ── Builds the outgoing SOAP envelope ─────────────────────────────────
    private static string BuildRequest(string countryCode, string vatNumber) => $"""
        <?xml version="1.0" encoding="UTF-8"?>
        <soapenv:Envelope
            xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
            xmlns:urn="{Namespace}">
          <soapenv:Header/>
          <soapenv:Body>
            <urn:checkVat>
              <urn:countryCode>{countryCode}</urn:countryCode>
              <urn:vatNumber>{vatNumber}</urn:vatNumber>
            </urn:checkVat>
          </soapenv:Body>
        </soapenv:Envelope>
        """;

    // ── Parses the VIES SOAP response ──────────────────────────────────────
    private static ViesResult ParseResponse(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);

        var ns = new XmlNamespaceManager(doc.NameTable);
        ns.AddNamespace("urn", Namespace);

        var node = doc.SelectSingleNode("//urn:checkVatResponse", ns)
            ?? throw new InvalidOperationException("Unexpected VIES response structure.");

        return new ViesResult
        {
            CountryCode = node.SelectSingleNode("urn:countryCode", ns)?.InnerText ?? "",
            VatNumber   = node.SelectSingleNode("urn:vatNumber",   ns)?.InnerText ?? "",
            IsValid     = node.SelectSingleNode("urn:valid",       ns)?.InnerText == "true",
            Name        = node.SelectSingleNode("urn:name",        ns)?.InnerText,
            Address     = node.SelectSingleNode("urn:address",     ns)?.InnerText,
            RequestDate = DateTime.TryParse(
                              node.SelectSingleNode("urn:requestDate", ns)?.InnerText,
                              out var dt) ? dt : DateTime.UtcNow
        };
    }
}