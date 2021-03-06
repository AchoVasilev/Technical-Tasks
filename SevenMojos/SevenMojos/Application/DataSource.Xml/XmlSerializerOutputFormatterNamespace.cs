namespace DataSource.Xml;

using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;

public class XmlSerializerOutputFormatterNamespace : XmlSerializerOutputFormatter
{
    protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object value)
    {
        //applying "empty" namespace will produce no namespaces
        var emptyNamespaces = new XmlSerializerNamespaces();
        emptyNamespaces.Add("", "any-non-empty-string");
        xmlSerializer.Serialize(xmlWriter, value, emptyNamespaces);
    }
}