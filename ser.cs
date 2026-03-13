using System.Xml;
using System.Xml.Serialization;

// ─── Namespaces ────────────────────────────────────────────────────────────
// UBL uses three XML namespaces. XmlSerializer needs them declared at the
// root so child elements inherit them — no xmlns= noise on every element.
// ──────────────────────────────────────────────────────────────────────────

[XmlRoot("Invoice", Namespace = Namespaces.Invoice)]
public class PeppolInvoice
{
    // BT-24: always this exact value for Peppol BIS 3.0
    [XmlElement("CustomizationID", Namespace = Namespaces.Cbc)]
    public string CustomizationID { get; set; } =
        "urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0";

    // BT-23: always this exact value
    [XmlElement("ProfileID", Namespace = Namespaces.Cbc)]
    public string ProfileID { get; set; } =
        "urn:fdc:peppol.eu:2017:poacc:billing:01:1.0";

    // BT-1: invoice number
    [XmlElement("ID", Namespace = Namespaces.Cbc)]
    public string InvoiceNumber { get; set; } = default!;

    // BT-2: issue date as string (format: YYYY-MM-DD)
    [XmlElement("IssueDate", Namespace = Namespaces.Cbc)]
    public string IssueDate { get; set; } = default!;

    // BT-3: 380 = commercial invoice
    [XmlElement("InvoiceTypeCode", Namespace = Namespaces.Cbc)]
    public string InvoiceTypeCode { get; set; } = "380";

    // BT-5: currency code e.g. DKK
    [XmlElement("DocumentCurrencyCode", Namespace = Namespaces.Cbc)]
    public string Currency { get; set; } = default!;

    // BT-10: buyer reference — mandatory for Danish invoices
    [XmlElement("BuyerReference", Namespace = Namespaces.Cbc)]
    public string BuyerReference { get; set; } = default!;

    // BT-27 / BT-31: supplier
    [XmlElement("AccountingSupplierParty", Namespace = Namespaces.Cac)]
    public AccountingParty Supplier { get; set; } = default!;

    // BT-44 / BT-47: buyer
    [XmlElement("AccountingCustomerParty", Namespace = Namespaces.Cac)]
    public AccountingParty Buyer { get; set; } = default!;

    // BT-110: total VAT amount
    [XmlElement("TaxTotal", Namespace = Namespaces.Cac)]
    public TaxTotal TaxTotal { get; set; } = default!;

    // BT-106 / BT-112 / BT-115: monetary totals
    [XmlElement("LegalMonetaryTotal", Namespace = Namespaces.Cac)]
    public LegalMonetaryTotal MonetaryTotal { get; set; } = default!;

    // At least one line is required by the spec
    [XmlElement("InvoiceLine", Namespace = Namespaces.Cac)]
    public List<InvoiceLine> Lines { get; set; } = new();
}

// ─── Party ────────────────────────────────────────────────────────────────

public class AccountingParty
{
    [XmlElement("Party", Namespace = Namespaces.Cac)]
    public Party Party { get; set; } = default!;
}

public class Party
{
    [XmlElement("PartyName", Namespace = Namespaces.Cac)]
    public PartyName PartyName { get; set; } = default!;

    [XmlElement("PartyTaxScheme", Namespace = Namespaces.Cac)]
    public PartyTaxScheme TaxScheme { get; set; } = default!;
}

public class PartyName
{
    // BT-27 (supplier) / BT-44 (buyer)
    [XmlElement("Name", Namespace = Namespaces.Cbc)]
    public string Name { get; set; } = default!;
}

public class PartyTaxScheme
{
    // BT-31 (supplier CVR) / BT-47 (buyer CVR) — format: DK + 8 digits
    [XmlElement("CompanyID", Namespace = Namespaces.Cbc)]
    public string CompanyID { get; set; } = default!;

    [XmlElement("TaxScheme", Namespace = Namespaces.Cac)]
    public TaxScheme TaxScheme { get; set; } = new();
}

public class TaxScheme
{
    [XmlElement("ID", Namespace = Namespaces.Cbc)]
    public string ID { get; set; } = "VAT";
}

// ─── Tax total ────────────────────────────────────────────────────────────

public class TaxTotal
{
    // BT-110
    [XmlElement("TaxAmount", Namespace = Namespaces.Cbc)]
    public CurrencyAmount TaxAmount { get; set; } = default!;
}

// ─── Monetary totals ──────────────────────────────────────────────────────

public class LegalMonetaryTotal
{
    // BT-106: net amount excluding VAT
    [XmlElement("TaxExclusiveAmount", Namespace = Namespaces.Cbc)]
    public CurrencyAmount NetAmount { get; set; } = default!;

    // BT-112: gross amount including VAT
    [XmlElement("TaxInclusiveAmount", Namespace = Namespaces.Cbc)]
    public CurrencyAmount GrossAmount { get; set; } = default!;

    // BT-115: amount due for payment
    [XmlElement("PayableAmount", Namespace = Namespaces.Cbc)]
    public CurrencyAmount PayableAmount { get; set; } = default!;
}

// ─── Invoice line ─────────────────────────────────────────────────────────

public class InvoiceLine
{
    [XmlElement("ID", Namespace = Namespaces.Cbc)]
    public string LineID { get; set; } = default!;

    [XmlElement("InvoicedQuantity", Namespace = Namespaces.Cbc)]
    public InvoicedQuantity Quantity { get; set; } = default!;

    [XmlElement("LineExtensionAmount", Namespace = Namespaces.Cbc)]
    public CurrencyAmount LineAmount { get; set; } = default!;

    [XmlElement("Item", Namespace = Namespaces.Cac)]
    public Item Item { get; set; } = default!;

    [XmlElement("Price", Namespace = Namespaces.Cac)]
    public Price Price { get; set; } = default!;
}

public class InvoicedQuantity
{
    [XmlAttribute("unitCode")]
    public string UnitCode { get; set; } = "HUR";

    [XmlText]
    public decimal Value { get; set; }
}

public class Item
{
    [XmlElement("Name", Namespace = Namespaces.Cbc)]
    public string Name { get; set; } = default!;
}

public class Price
{
    [XmlElement("PriceAmount", Namespace = Namespaces.Cbc)]
    public CurrencyAmount UnitPrice { get; set; } = default!;
}

// ─── Shared type: amounts with currencyID attribute ───────────────────────

public class CurrencyAmount
{
    [XmlAttribute("currencyID")]
    public string CurrencyID { get; set; } = "DKK";

    [XmlText]
    public decimal Value { get; set; }
}

// ─── Namespace constants ──────────────────────────────────────────────────

public static class Namespaces
{
    public const string Invoice = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
    public const string Cac     = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
    public const string Cbc     = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
}