// =============================================================================
//  Danish SAF-T Financial - C# Serialization Models
//  Namespace: urn:StandardAuditFile-Taxation-Financial:DK
//  Version: 2.0 (expanded for three-tier fraud detection demo)
//  Based on: Danish SAF-T Financial Data v1.0 (Erhvervsstyrelsen / OECD SAF-T v2)
//
//  Tier 1 additions: SourceID, BatchID, DebitCreditIndicator, GoodsServicesID,
//                    Supplier.RegistrationDate, GeneralLedgerEntries classes
//  Tier 2 additions: SystemEntryDate (DateTime, not DateOnly) on Invoice and
//                    GlTransaction — required for off-hours detection
//  Tier 3 additions: PeppolDocumentID on Invoice and GlTransaction for
//                    SAF-T <-> Peppol cross-source matching
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

        /// <summary>
        /// Tier 1: TransactionID sequence gap detection — every invoice
        /// TransactionID must map to a GL transaction here. Gaps or orphans
        /// flag suppression.
        /// Tier 2: GL line amounts are the primary input for Benford's Law
        /// and round-number clustering analysis.
        /// </summary>
        [XmlElement("GeneralLedgerEntries")]
        public GeneralLedgerEntries? GeneralLedgerEntries { get; set; }

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

        /// <summary>
        /// Tier 1: CVR registration date of this supplier.
        /// Flag: supplier registered less than 90 days before their first invoice
        /// is a phantom/missing-trader risk signal.
        /// </summary>
        [XmlElement("RegistrationDate", DataType = "date")]
        public DateTime? RegistrationDate { get; set; }
    }

    // =========================================================================
    //  GENERAL LEDGER ENTRIES
    //  Tier 1: sequence gap detection on TransactionID within each Journal.
    //  Tier 2: DebitAmount/CreditAmount values feed Benford's Law analysis.
    // =========================================================================

    public class GeneralLedgerEntries
    {
        /// <summary>Total number of GL transaction lines across all journals.</summary>
        [XmlElement("NumberOfEntries")]
        public int? NumberOfEntries { get; set; }

        [XmlElement("TotalDebit")]
        public decimal? TotalDebit { get; set; }

        [XmlElement("TotalCredit")]
        public decimal? TotalCredit { get; set; }

        [XmlElement("Journal")]
        public List<Journal> Journals { get; set; } = new();
    }

    public class Journal
    {
        /// <summary>Journal identifier, e.g. "SALG" (Sales), "KOB" (Purchases), "MEM" (Memorial).</summary>
        [XmlElement("JournalID")]
        public string JournalID { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string? Description { get; set; }

        /// <summary>Journal type, e.g. "Sales", "Purchase", "Bank", "General".</summary>
        [XmlElement("Type")]
        public string? Type { get; set; }

        [XmlElement("Transaction")]
        public List<GlTransaction> Transactions { get; set; } = new();
    }

    /// <summary>
    /// A single GL transaction (journal voucher / bilag).
    ///
    /// Tier 1: TransactionID must be unique and sequential within a Journal.
    ///         Gaps in the sequence signal suppressed entries.
    ///         SourceDocumentID on each Line links back to invoice InvoiceNo —
    ///         invoices with no matching GL entry are phantom/suppressed.
    ///
    /// Tier 2: SystemEntryDate (DateTime) enables off-hours posting detection.
    ///         SourceID enables operator-level anomaly clustering.
    /// </summary>
    public class GlTransaction
    {
        /// <summary>
        /// Tier 1: primary key for sequence gap analysis.
        /// Must form a gapless sequence within each Journal (e.g. 1, 2, 3 — not 1, 3, 5).
        /// </summary>
        [XmlElement("TransactionID")]
        public string TransactionID { get; set; } = string.Empty;

        /// <summary>Accounting month (1–12).</summary>
        [XmlElement("Period")]
        public int Period { get; set; }

        [XmlElement("PeriodYear")]
        public int PeriodYear { get; set; }

        /// <summary>Document date (invoice or voucher date).</summary>
        [XmlElement("TransactionDate", DataType = "date")]
        public DateTime TransactionDate { get; set; }

        /// <summary>Date the transaction was posted to the GL.</summary>
        [XmlElement("GLPostingDate", DataType = "date")]
        public DateTime? GLPostingDate { get; set; }

        /// <summary>
        /// Tier 2: exact timestamp the record was entered in the accounting system.
        /// Format: YYYY-MM-DDTHH:MM:SS (e.g. 2025-01-15T02:17:43).
        /// The time component is essential for off-hours posting detection.
        /// A posting at 02:17 AM is statistically anomalous and a Tier 2 signal.
        /// </summary>
        [XmlElement("SystemEntryDate")]
        public DateTime? SystemEntryDate { get; set; }

        /// <summary>
        /// Tier 1/2: ID of the user or system that created this transaction.
        /// Unexpected SourceIDs (e.g. a terminated employee's login) on
        /// high-value entries are a Tier 1 phantom-vendor indicator.
        /// Clustering by SourceID reveals off-hours operator patterns (Tier 2).
        /// </summary>
        [XmlElement("SourceID")]
        public string? SourceID { get; set; }

        [XmlElement("Description")]
        public string? Description { get; set; }

        /// <summary>Tier 2: system batch reference for off-hours batch clustering.</summary>
        [XmlElement("BatchID")]
        public string? BatchID { get; set; }

        /// <summary>
        /// Tier 3: Peppol BIS 3.0 document ID of the corresponding e-invoice.
        /// A GL entry referencing a Peppol ID that doesn't exist in the Peppol
        /// network (or vice versa) is a Tier 3 suppression signal.
        /// </summary>
        [XmlElement("PeppolDocumentID")]
        public string? PeppolDocumentID { get; set; }

        [XmlElement("Line")]
        public List<GlTransactionLine> Lines { get; set; } = new();
    }

    /// <summary>
    /// A single line within a GL transaction (one side of a double entry).
    /// All lines within a GlTransaction must balance: ΣDebits = ΣCredits.
    ///
    /// Tier 2: the Amount values here (independent of invoice totals) are the
    /// primary input for Benford's Law digit frequency analysis and
    /// round-number clustering.
    /// </summary>
    public class GlTransactionLine
    {
        /// <summary>Line identifier within the transaction, e.g. "1", "2".</summary>
        [XmlElement("RecordID")]
        public string RecordID { get; set; } = string.Empty;

        /// <summary>GL account code (e.g. "4000", "2630").</summary>
        [XmlElement("AccountID")]
        public string AccountID { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string? Description { get; set; }

        [XmlElement("CustomerID")]
        public string? CustomerID { get; set; }

        [XmlElement("SupplierID")]
        public string? SupplierID { get; set; }

        /// <summary>
        /// Reference back to the source invoice (InvoiceNo).
        /// Tier 1: a GL line with no matching SourceDocumentID in the invoice
        /// collections is a phantom entry; an invoice with no GL line referencing
        /// it is a suppressed entry.
        /// </summary>
        [XmlElement("SourceDocumentID")]
        public string? SourceDocumentID { get; set; }

        /// <summary>
        /// Debit side of the entry. Populate either DebitAmount or CreditAmount,
        /// not both, on any single line.
        /// Tier 2: Amount feeds Benford's Law analysis.
        /// </summary>
        [XmlElement("DebitAmount")]
        public CurrencyAmount? DebitAmount { get; set; }

        /// <summary>
        /// Credit side of the entry. Populate either DebitAmount or CreditAmount,
        /// not both, on any single line.
        /// Tier 2: Amount feeds Benford's Law analysis.
        /// </summary>
        [XmlElement("CreditAmount")]
        public CurrencyAmount? CreditAmount { get; set; }

        [XmlElement("TaxInformation")]
        public List<TaxInformation> TaxInformation { get; set; } = new();
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

        [XmlElement("SpecialCircumstances")]
        public string? SpecialCircumstances { get; set; }

        [XmlElement("SystemID")]
        public string? SystemID { get; set; }

        [XmlElement("GLPostingDate", DataType = "date")]
        public DateTime? GLPostingDate { get; set; }

        /// <summary>Cross-reference to GeneralLedgerEntries.Journal.Transaction.TransactionID.</summary>
        [XmlElement("TransactionID")]
        public string? TransactionID { get; set; }

        /// <summary>
        /// Tier 1: ID of the user or system that entered this invoice.
        /// Unexpected SourceIDs on high-value purchase invoices are a
        /// phantom-vendor indicator (e.g. entry by a non-AP user).
        /// </summary>
        [XmlElement("SourceID")]
        public string? SourceID { get; set; }

        /// <summary>
        /// Tier 1/2: system batch reference.
        /// Used to cluster entries and detect off-hours batch submissions.
        /// </summary>
        [XmlElement("BatchID")]
        public string? BatchID { get; set; }

        /// <summary>
        /// Tier 2: exact timestamp the invoice was entered into the accounting system.
        /// The time component (HH:MM:SS) is what makes off-hours detection possible —
        /// xs:date alone (InvoiceDate, GLPostingDate) cannot catch a 2 AM posting.
        /// Example: 2025-01-15T02:17:43
        /// </summary>
        [XmlElement("SystemEntryDate")]
        public DateTime? SystemEntryDate { get; set; }

        /// <summary>
        /// Tier 3: Peppol BIS 3.0 document ID from the corresponding e-invoice.
        /// Cross-match logic:
        ///   1. InvoiceNo (SAF-T)  ==  ID (Peppol UBL)
        ///   2. SupplierTaxID      ==  AccountingSupplierParty/PartyTaxScheme/CompanyID
        ///   3. GrossTotal         ==  TaxInclusiveAmount
        /// A purchase invoice with no matching PeppolDocumentID where one is expected
        /// signals invoice suppression (Tier 3, strongest differentiator).
        /// A match where GrossTotal differs signals amount tampering (Tier 3).
        /// </summary>
        [XmlElement("PeppolDocumentID")]
        public string? PeppolDocumentID { get; set; }

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

        /// <summary>
        /// Tier 1: "D" (Debit) or "C" (Credit).
        /// Enables double-entry sum verification: all D lines minus all C lines
        /// across a transaction must net to zero. A mismatch indicates manipulation.
        /// </summary>
        [XmlElement("DebitCreditIndicator")]
        public string? DebitCreditIndicator { get; set; }

        /// <summary>
        /// Tier 1: "Goods" or "Services".
        /// Used to verify tax code consistency — e.g. a line coded as "Goods"
        /// should not carry an exempt TaxCode if the goods are standard-rated.
        /// Mismatches between GoodsServicesID and the StandardTaxCode in the
        /// TaxTable are a tax misclassification signal.
        /// </summary>
        [XmlElement("GoodsServicesID")]
        public string? GoodsServicesID { get; set; }

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
//  USAGE EXAMPLE: three-tier validation queries in your API controller
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
//      var f = (AuditFile?)serializer.Deserialize(stream);
//      if (f is null) return BadRequest("Could not parse SAF-T file.");
//
//      var sales     = f.SourceDocuments?.SalesInvoices?.Invoices    ?? new();
//      var purchases = f.SourceDocuments?.PurchaseInvoices?.Invoices ?? new();
//      var glTxns    = f.GeneralLedgerEntries?.Journals
//                       .SelectMany(j => j.Transactions).ToList()   ?? new();
//
//      // -----------------------------------------------------------------------
//      //  VAT ASSESSMENT (existing)
//      // -----------------------------------------------------------------------
//      decimal outputVat = sales.SelectMany(i => i.Lines)
//                               .SelectMany(l => l.TaxInformation)
//                               .Where(t => t.TaxType == "VAT")
//                               .Sum(t => t.TaxAmount.Amount);
//
//      decimal inputVat  = purchases.SelectMany(i => i.Lines)
//                                   .SelectMany(l => l.TaxInformation)
//                                   .Where(t => t.TaxType == "VAT")
//                                   .Sum(t => t.TaxAmount.Amount);
//
//      decimal netVatLiability = outputVat - inputVat;
//
//      // -----------------------------------------------------------------------
//      //  TIER 1: Internal SAF-T consistency
//      // -----------------------------------------------------------------------
//
//      // --- 1a. Invoice TransactionID sequence gaps ---
//      //  Every TransactionID on an invoice should exist in GeneralLedgerEntries.
//      //  Missing GL transactions are suppression candidates.
//      var glTxnIds = glTxns.Select(t => t.TransactionID).ToHashSet();
//      var orphanInvoices = sales.Concat(purchases)
//          .Where(i => i.TransactionID != null && !glTxnIds.Contains(i.TransactionID!))
//          .Select(i => i.InvoiceNo)
//          .ToList();
//      // orphanInvoices: invoices with a TransactionID that has no GL record
//
//      // --- 1b. Phantom suppliers (recently registered) ---
//      var cutoff = f.Header.SelectionStartDate.AddDays(-90);
//      var phantomSuppliers = f.MasterFiles?.Suppliers?.SupplierList
//          .Where(s => s.RegistrationDate.HasValue && s.RegistrationDate > cutoff)
//          .Select(s => new { s.SupplierID, s.CompanyName, s.RegistrationDate })
//          .ToList();
//
//      // --- 1c. Tax misclassification: effective rate vs. declared TaxCode ---
//      var misclassified = sales.Concat(purchases)
//          .SelectMany(i => i.Lines)
//          .SelectMany(l => l.TaxInformation.Select(t => new { Line = l, Tax = t }))
//          .Where(x => x.Tax.TaxBase.HasValue && x.Tax.TaxBase > 0
//                   && x.Tax.TaxPercentage.HasValue
//                   && Math.Abs(x.Tax.TaxAmount.Amount / x.Tax.TaxBase.Value
//                               - x.Tax.TaxPercentage.Value / 100m) > 0.001m)
//          .ToList();
//
//      // --- 1d. Double-entry balance check per GL transaction ---
//      var unbalanced = glTxns
//          .Where(t => {
//              decimal debits  = t.Lines.Sum(l => l.DebitAmount?.Amount  ?? 0m);
//              decimal credits = t.Lines.Sum(l => l.CreditAmount?.Amount ?? 0m);
//              return Math.Abs(debits - credits) > 0.01m;
//          })
//          .Select(t => t.TransactionID)
//          .ToList();
//
//      // -----------------------------------------------------------------------
//      //  TIER 2: Statistical anomaly detection
//      // -----------------------------------------------------------------------
//
//      // --- 2a. Off-hours posting detection (requires SystemEntryDate time component) ---
//      var offHours = sales.Concat(purchases)
//          .Where(i => i.SystemEntryDate.HasValue
//                   && (i.SystemEntryDate.Value.Hour < 6 || i.SystemEntryDate.Value.Hour >= 22))
//          .Select(i => new {
//              i.InvoiceNo,
//              EntryTime = i.SystemEntryDate!.Value.ToString("HH:mm"),
//              i.SourceID
//          })
//          .ToList();
//
//      // --- 2b. GL-level off-hours (same logic on GL transactions) ---
//      var glOffHours = glTxns
//          .Where(t => t.SystemEntryDate.HasValue
//                   && (t.SystemEntryDate.Value.Hour < 6 || t.SystemEntryDate.Value.Hour >= 22))
//          .ToList();
//
//      // --- 2c. Round-number clustering (Tier 2 flag: round amounts at high frequency) ---
//      var glAmounts = glTxns.SelectMany(t => t.Lines)
//          .Select(l => l.DebitAmount?.Amount ?? l.CreditAmount?.Amount ?? 0m)
//          .Where(a => a > 0)
//          .ToList();
//      double roundNumberRatio = glAmounts.Count > 0
//          ? (double)glAmounts.Count(a => a % 1000 == 0) / glAmounts.Count
//          : 0;
//      // Flag if > 15% of GL amounts are exact multiples of 1,000
//
//      // -----------------------------------------------------------------------
//      //  TIER 3: Cross-source SAF-T <-> Peppol matching
//      // -----------------------------------------------------------------------
//
//      // --- 3a. Purchase invoices that should have a Peppol ID but don't ---
//      //  (in practice: purchases above a threshold from VAT-registered suppliers)
//      var missingPeppol = purchases
//          .Where(i => string.IsNullOrEmpty(i.PeppolDocumentID))
//          .Select(i => new { i.InvoiceNo, i.SupplierID, i.DocumentTotals.GrossTotal })
//          .ToList();
//
//      // --- 3b. Cross-match: GrossTotal in SAF-T vs. Peppol TaxInclusiveAmount ---
//      //  (assumes caller resolves peppolInvoices from Peppol network lookup)
//      //  Example: PeppolInvoice has { DocumentID, TaxInclusiveAmount }
//      //
//      //  var tampered = purchases
//      //      .Where(i => !string.IsNullOrEmpty(i.PeppolDocumentID))
//      //      .Join(peppolInvoices,
//      //            s => s.PeppolDocumentID,
//      //            p => p.DocumentID,
//      //            (s, p) => new { s.InvoiceNo, SaftGross = s.DocumentTotals.GrossTotal,
//      //                            PeppolGross = p.TaxInclusiveAmount })
//      //      .Where(x => Math.Abs(x.SaftGross - x.PeppolGross) > 0.01m)
//      //      .ToList();
//      //  tampered: amount-tampered invoices (SAF-T shows different gross than Peppol)
//
//      return Ok(new {
//          // VAT assessment
//          Company          = f.Header.Company.RegistrationNumber,
//          Period           = $"{f.Header.SelectionStartDate:yyyy-MM-dd} to {f.Header.SelectionEndDate:yyyy-MM-dd}",
//          OutputVAT        = outputVat,
//          InputVAT         = inputVat,
//          NetLiability     = netVatLiability,
//          PayableToSkat    = netVatLiability > 0 ? netVatLiability : 0,
//          RefundDue        = netVatLiability < 0 ? Math.Abs(netVatLiability) : 0,
//          // Tier 1
//          Tier1_OrphanInvoices    = orphanInvoices,
//          Tier1_PhantomSuppliers  = phantomSuppliers,
//          Tier1_Misclassified     = misclassified.Count,
//          Tier1_UnbalancedEntries = unbalanced,
//          // Tier 2
//          Tier2_OffHoursInvoices  = offHours,
//          Tier2_GlOffHoursCount   = glOffHours.Count,
//          Tier2_RoundNumberRatio  = roundNumberRatio,
//          // Tier 3
//          Tier3_MissingPeppolIDs  = missingPeppol,
//      });
//  }
//
// =============================================================================
