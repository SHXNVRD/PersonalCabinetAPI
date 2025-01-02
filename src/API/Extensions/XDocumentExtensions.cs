using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace API.Extensions;

public static class XDocumentExtensions
{
    public static byte[] ToByteArray(this XDocument xml, Encoding encoding)
    {
        using var memory = new MemoryStream();
        using var writer = XmlWriter.Create(memory, new XmlWriterSettings
        {
            Encoding = encoding
        });
        
        xml.Save(writer);
        writer.Flush();
        return memory.ToArray();
    }
}