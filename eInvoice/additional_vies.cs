public class Party
{
    [XmlElement("PartyName",        Namespace = Namespaces.Cac)]
    public PartyName PartyName { get; set; } = default!;

    [XmlElement("PostalAddress",    Namespace = Namespaces.Cac)]
    public PostalAddress PostalAddress { get; set; } = default!;

    [XmlElement("PartyTaxScheme",   Namespace = Namespaces.Cac)]
    public PartyTaxScheme TaxScheme { get; set; } = default!;

    [XmlElement("PartyLegalEntity", Namespace = Namespaces.Cac)]  // ← add this
    public PartyLegalEntity LegalEntity { get; set; } = default!;
}

public class PartyLegalEntity                                      // ← add this class
{
    [XmlElement("RegistrationName", Namespace = Namespaces.Cbc)]
    public string? RegistrationName { get; set; }

    // CVR for Danish companies, Handelsregisternummer for German, etc.
    [XmlElement("CompanyID",        Namespace = Namespaces.Cbc)]
    public string CompanyID { get; set; } = default!;
}

public class PostalAddress                                       // ← add this class
{
    [XmlElement("StreetName",   Namespace = Namespaces.Cbc)]
    public string? Street { get; set; }

    [XmlElement("CityName",     Namespace = Namespaces.Cbc)]
    public string? City { get; set; }

    [XmlElement("PostalZone",   Namespace = Namespaces.Cbc)]
    public string? PostCode { get; set; }

    [XmlElement("Country",      Namespace = Namespaces.Cac)]
    public Country Country { get; set; } = default!;
}

public class Country                                             // ← add this class
{
    // ISO 3166-1 alpha-2 — e.g. DK, DE, SE
    // This is what you split to get the VIES country code
    [XmlElement("IdentificationCode", Namespace = Namespaces.Cbc)]
    public string Code { get; set; } = default!;
}