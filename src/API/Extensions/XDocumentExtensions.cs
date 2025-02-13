using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace API.Extensions;

public static class XDocumentExtensions
{
    public static async Task<byte[]> ToByteArrayAsync(this XDocument xml, Encoding encoding, CancellationToken cancellationToken = default)
    {
        using var memory = new MemoryStream();
        await using var writer = XmlWriter.Create(memory, new XmlWriterSettings
        {
            Async = true,
            Encoding = encoding
        });
        
        await xml.SaveAsync(writer, cancellationToken);
        await writer.FlushAsync();
        return memory.ToArray();
    }

    public static async Task<XDocument> ToXDocumentAsync(this byte[] buffer, CancellationToken cancellationToken = default)
    {
        using var memory = new MemoryStream(buffer);
        using var reader = XmlReader.Create(memory, new XmlReaderSettings
        {
            Async = true,
        });
        
        return await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken);
    }
}