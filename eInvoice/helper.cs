public static class PeppolSerializer
{
    private static readonly XmlSerializer _serializer = new(typeof(PeppolInvoice));

    // Declare all three namespaces at the root — prevents xmlns= on
    // every child element, which would still be valid XML but looks ugly
    private static readonly XmlSerializerNamespaces _ns = BuildNamespaces();

    private static XmlSerializerNamespaces BuildNamespaces()
    {
        var ns = new XmlSerializerNamespaces();
        ns.Add("",    Namespaces.Invoice);   // default namespace (no prefix)
        ns.Add("cac", Namespaces.Cac);
        ns.Add("cbc", Namespaces.Cbc);
        return ns;
    }

    public static PeppolInvoice Deserialize(Stream xmlStream)
    {
        var result = _serializer.Deserialize(xmlStream)
            ?? throw new InvalidOperationException("Deserialization returned null.");
        return (PeppolInvoice)result;
    }

    public static string Serialize(PeppolInvoice invoice)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = System.Text.Encoding.UTF8,
            OmitXmlDeclaration = false
        };

        using var sw = new StringWriter();
        using var writer = XmlWriter.Create(sw, settings);
        _serializer.Serialize(writer, invoice, _ns);
        return sw.ToString();
    }
}