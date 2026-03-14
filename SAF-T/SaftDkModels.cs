// =============================================================================
//  Danish SAF-T Financial - C# Serialization Models
//  Namespace: urn:StandardAuditFile-Taxation-Financial:DK
//  Version: 1.0 (Simplified Demo)
//  Based on: Danish SAF-T Financial Data v1.0 (Erhvervsstyrelsen / OECD SAF-T v2)
// =============================================================================

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Skat.SaftDk.Models
{
    // =========================================================================
    //  ROOT: AuditFile
    // =========================================================================

    [XmlRoot("AuditFile",
        Namespace = "urn:StandardAuditFile-Taxation-Financial:DK")]
    public class AuditFile
    {
        [XmlElement("Header")]
        public AuditFileHeader Header { get; set; } = new();

        [XmlElement("MasterFiles")]
        public MasterFiles? MasterFiles { get; set; }

        [XmlElement("SourceDocuments")]
        public SourceDocuments? SourceDocuments { get; set; }
    }

    // =========================================================================
    //  HEADER
    // =========================================================================

    public class AuditFileHeader
    {
        /// <summary>Version of the SAF-T standard used. E.g. "1.0"</summary>
        [XmlElement("AuditFileVersion")]
        public string AuditFileVersion { get; set; } = "1.0";

        /// <summary>ISO 3166-1 alpha-2 country code. Always "DK" for Denmark.</summary>
        [XmlElement("AuditFileCountry")]
        public string AuditFileCountry { get; set; } = "DK";

        /// <summary>Date the SAF-T file was generated.</summary>
        [XmlElement("AuditFileDateCreated", DataType = "date")]
        public DateTime AuditFileDateCreated { get; set; }

        [XmlElement("SoftwareCompanyName")]
        public string? SoftwareCompanyName { get; set; }

        [XmlElement("SoftwareID")]
        public string? SoftwareID { get; set; }

        [XmlElement("SoftwareVersion")]
        public string? SoftwareVersion { get; set; }

        [XmlElement("Company")]
        public CompanyHeader Company { get; set; } = new();

        /// <summary>ISO 4217 currency code. Default "DKK".</summary>
        [XmlElement("DefaultCurrencyCode")]
        public string DefaultCurrencyCode { get; set; } = "DKK";

        /// <summary>Start of the reporting period (e.g. 2025-01-01)</summary>
        [XmlElement("SelectionStartDate", DataType = "date")]
        public DateTime SelectionStartDate { get; set; }

        /// <summary>End of the reporting period (e.g. 2025-03-31)</summary>
        [XmlElement("SelectionEndDate", DataType = "date")]
        public DateTime SelectionEndDate { get; set; }

        [XmlElement("HeaderComment")]
        public string? HeaderComment { get; set; }

        /// <summary>Fiscal year in format YYYY (or YYYY-YYYY for split years)</summary>
        [XmlElement("FiscalYear")]
        public string? FiscalYear { get; set; }

        [XmlElement("NumberOfParts")]
        public int? NumberOfParts { get; set; }

        [XmlElement("PartNumber")]
        public int? PartNumber { get; set; }
    }

    // =========================================================================
    //  COMPANY HEADER
    // =========================================================================

    public class CompanyHeader
    {
        /// <summary>Danish CVR number – exactly 8 digits</summary>
        [XmlElement("RegistrationNumber")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Address")]
        public Address? Address { get; set; }

        [XmlElement("TaxRegistration")]
        public TaxRegistration TaxRegistration { get; set; } = new();
    }

    public class TaxRegistration
    {
        /// <summary>Danish CVR number (same as RegistrationNumber for DK companies)</summary>
        [XmlElement("TaxRegistrationNumber")]
        public string TaxRegistrationNumber { get; set; } = string.Empty;

        /// <summary>Tax type. Use "VAT" for Moms.</summary>
        [XmlElement("TaxType")]
        public string TaxType { get; set; } = "VAT";

        [XmlElement("TaxAuthority")]
        public string? TaxAuthority { get; set; }
    }

    // =========================================================================
    //  SHARED: ADDRESS
    // =========================================================================

    public class Address
    {
        [XmlElement("StreetName")]
        public string? StreetName { get; set; }

        [XmlElement("AdditionalInfo")]
        public string? AdditionalInfo { get; set; }

        [XmlElement("PostalCode")]
        public string? PostalCode { get; set; }

        [XmlElement("City")]
        public string? City { get; set; }

        /// <summary>ISO 3166-1 alpha-2 country code</summary>
        [XmlElement("Country")]
        public string? Country { get; set; }
    }

    // =========================================================================
    //  MASTER FILES
    // =========================================================================

    public class MasterFiles
    {
        [XmlElement("TaxTable")]
        public TaxTable? TaxTable { get; set; }

        [XmlElement("Customers")]
        public Customers? Customers { get; set; }

        [XmlElement("Suppliers")]
        public Suppliers? Suppliers { get; set; }
    }

    // --- Tax Table ---

    public class TaxTable
    {
        [XmlElement("TaxTableEntry")]
        public List<TaxCodeDetails> TaxTableEntries { get; set; } = new();
    }

    public class TaxCodeDetails
    {
        /// <summary>Internal tax code (e.g. "MOMS25")</summary>
        [XmlElement("TaxCode")]
        public string TaxCode { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string? Description { get; set; }

        /// <summary>"VAT" or "OTHER"</summary>
        [XmlElement("TaxType")]
        public string TaxType { get; set; } = "VAT";

        /// <summary>
        /// Standard Danish VAT code per SAF-T DK code list:
        /// 1 = Standard 25%, 3 = Zero-rated, 4 = Exempt, 5 = Reverse charge, etc.
        /// </summary>
        [XmlElement("StandardTaxCode")]
        public string StandardTaxCode { get; set; } = string.Empty;

        [XmlElement("Country")]
        public string? Country { get; set; }

        [XmlElement("TaxPercentage")]
        public decimal? TaxPercentage { get; set; }
    }

    // --- Customers ---

    public class Customers
    {
        [XmlElement("Customer")]
        public List<Customer> CustomerList { get; set; } = new();
    }

    public class Customer
    {
        [XmlElement("CustomerID")]
        public string CustomerID { get; set; } = string.Empty;

        [XmlElement("AccountID")]
        public string? AccountID { get; set; }

        [XmlElement("CustomerTaxID")]
        public string? CustomerTaxID { get; set; }

        [XmlElement("CompanyName")]
        public string? CompanyName { get; set; }

        [XmlElement("BillingAddress")]
        public Address? BillingAddress { get; set; }
    }

    // --- Suppliers ---

    public class Suppliers
    {
        [XmlElement("Supplier")]
        public List<Supplier> SupplierList { get; set; } = new();
    }

    public class Supplier
    {
        [XmlElement("SupplierID")]
        public string SupplierID { get; set; } = string.Empty;

        [XmlElement("AccountID")]
        public string? AccountID { get; set; }

        [XmlElement("SupplierTaxID")]
        public string? SupplierTaxID { get; set; }

        [XmlElement("CompanyName")]
        public string? CompanyName { get; set; }

        [XmlElement("BillingAddress")]
        public Address? BillingAddress { get; set; }
    }

    // =========================================================================
    //  SOURCE DOCUMENTS
    // =========================================================================

    public class SourceDocuments
    {
        [XmlElement("SalesInvoices")]
        public InvoiceCollection? SalesInvoices { get; set; }

        [XmlElement("PurchaseInvoices")]
        public InvoiceCollection? PurchaseInvoices { get; set; }
    }

    public class InvoiceCollection
    {
        [XmlElement("NumberOfEntries")]
        public int? NumberOfEntries { get; set; }

        [XmlElement("TotalDebit")]
        public decimal? TotalDebit { get; set; }

        [XmlElement("TotalCredit")]
        public decimal? TotalCredit { get; set; }

        [XmlElement("Invoice")]
        public List<Invoice> Invoices { get; set; } = new();
    }

    // =========================================================================
    //  INVOICE
    // =========================================================================

    public class Invoice
    {
        [XmlElement("InvoiceNo")]
        public string InvoiceNo { get; set; } = string.Empty;

        /// <summary>Reference to Customer.CustomerID (for sales)</summary>
        [XmlElement("CustomerID")]
        public string? CustomerID { get; set; }

        /// <summary>Reference to Supplier.SupplierID (for purchases)</summary>
        [XmlElement("SupplierID")]
        public string? SupplierID { get; set; }

        /// <summary>Month number (1-12)</summary>
        [XmlElement("Period")]
        public int Period { get; set; }

        [XmlElement("PeriodYear")]
        public int PeriodYear { get; set; }

        [XmlElement("InvoiceDate", DataType = "date")]
        public DateTime InvoiceDate { get; set; }

        /// <summary>E.g. "Sale", "Purchase", "CreditNote"</summary>
        [XmlElement("InvoiceType")]
        public string? InvoiceType { get; set; }

        [XmlElement("SystemID")]
        public string? SystemID { get; set; }

        [XmlElement("GLPostingDate", DataType = "date")]
        public DateTime? GLPostingDate { get; set; }

        [XmlElement("TransactionID")]
        public string? TransactionID { get; set; }

        [XmlElement("Line")]
        public List<InvoiceLine> Lines { get; set; } = new();

        [XmlElement("DocumentTotals")]
        public DocumentTotals DocumentTotals { get; set; } = new();
    }

    // =========================================================================
    //  INVOICE LINE
    // =========================================================================

    public class InvoiceLine
    {
        [XmlElement("LineNumber")]
        public int LineNumber { get; set; }

        [XmlElement("AccountID")]
        public string? AccountID { get; set; }

        [XmlElement("ProductCode")]
        public string? ProductCode { get; set; }

        [XmlElement("Description")]
        public string? Description { get; set; }

        [XmlElement("Quantity")]
        public decimal? Quantity { get; set; }

        [XmlElement("UnitOfMeasure")]
        public string? UnitOfMeasure { get; set; }

        [XmlElement("UnitPrice")]
        public decimal? UnitPrice { get; set; }

        [XmlElement("TaxPointDate", DataType = "date")]
        public DateTime? TaxPointDate { get; set; }

        [XmlElement("InvoiceLineAmount")]
        public CurrencyAmount InvoiceLineAmount { get; set; } = new();

        [XmlElement("TaxInformation")]
        public List<TaxInformation> TaxInformation { get; set; } = new();
    }

    // =========================================================================
    //  TAX INFORMATION (on line and document totals)
    // =========================================================================

    public class TaxInformation
    {
        [XmlElement("TaxType")]
        public string TaxType { get; set; } = "VAT";

        /// <summary>Must reference a TaxCode in MasterFiles/TaxTable</summary>
        [XmlElement("TaxCode")]
        public string TaxCode { get; set; } = string.Empty;

        [XmlElement("TaxPercentage")]
        public decimal? TaxPercentage { get; set; }

        /// <summary>Net taxable base amount before VAT</summary>
        [XmlElement("TaxBase")]
        public decimal? TaxBase { get; set; }

        [XmlElement("TaxAmount")]
        public CurrencyAmount TaxAmount { get; set; } = new();
    }

    // =========================================================================
    //  DOCUMENT TOTALS
    // =========================================================================

    public class DocumentTotals
    {
        [XmlElement("TaxInformationTotals")]
        public List<TaxInformation> TaxInformationTotals { get; set; } = new();

        /// <summary>Total excluding VAT</summary>
        [XmlElement("NetTotal")]
        public decimal NetTotal { get; set; }

        /// <summary>Total including VAT</summary>
        [XmlElement("GrossTotal")]
        public decimal GrossTotal { get; set; }
    }

    // =========================================================================
    //  SHARED: CURRENCY AMOUNT
    // =========================================================================

    public class CurrencyAmount
    {
        /// <summary>Amount in default currency (DKK)</summary>
        [XmlElement("Amount")]
        public decimal Amount { get; set; }

        /// <summary>ISO 4217 currency code if foreign currency</summary>
        [XmlElement("CurrencyCode")]
        public string? CurrencyCode { get; set; }

        /// <summary>Amount in original foreign currency</summary>
        [XmlElement("CurrencyAmount")]
        public decimal? ForeignAmount { get; set; }

        [XmlElement("ExchangeRate")]
        public decimal? ExchangeRate { get; set; }
    }
}


// =============================================================================
//  USAGE EXAMPLE: Deserialize SAF-T XML in your API controller
// =============================================================================
//
//  using System.Xml.Serialization;
//  using Skat.SaftDk.Models;
//
//  [HttpPost("saft/upload")]
//  public async Task<IActionResult> Upload(IFormFile file)
//  {
//      var serializer = new XmlSerializer(typeof(AuditFile));
//      await using var stream = file.OpenReadStream();
//      var auditFile = (AuditFile?)serializer.Deserialize(stream);
//
//      if (auditFile is null)
//          return BadRequest("Could not parse SAF-T file.");
//
//      // --- VAT Assessment ---
//      var salesInvoices = auditFile.SourceDocuments?.SalesInvoices?.Invoices ?? new();
//      var purchaseInvoices = auditFile.SourceDocuments?.PurchaseInvoices?.Invoices ?? new();
//
//      decimal outputVat = salesInvoices
//          .SelectMany(i => i.Lines)
//          .SelectMany(l => l.TaxInformation)
//          .Where(t => t.TaxType == "VAT")
//          .Sum(t => t.TaxAmount.Amount);
//
//      decimal inputVat = purchaseInvoices
//          .SelectMany(i => i.Lines)
//          .SelectMany(l => l.TaxInformation)
//          .Where(t => t.TaxType == "VAT")
//          .Sum(t => t.TaxAmount.Amount);
//
//      decimal netVatLiability = outputVat - inputVat;
//
//      return Ok(new {
//          Company     = auditFile.Header.Company.RegistrationNumber,
//          Period      = $"{auditFile.Header.SelectionStartDate:yyyy-MM-dd} to {auditFile.Header.SelectionEndDate:yyyy-MM-dd}",
//          OutputVAT   = outputVat,
//          InputVAT    = inputVat,
//          NetLiability = netVatLiability,
//          PayableToSkat = netVatLiability > 0 ? netVatLiability : 0,
//          RefundDue   = netVatLiability < 0 ? Math.Abs(netVatLiability) : 0
//      });
//  }
//
// =============================================================================
