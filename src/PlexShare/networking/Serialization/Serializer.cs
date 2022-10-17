using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PlexShareNetworking.Serialization
{
    public class Serializer : ISerializer
    {
        public string Serialize<T>(T genericObject)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            StringWriter stringWriter = new StringWriter();
            XmlWriter xmlWriter = XmlWriter.Create(stringWriter);

            xmlSerializer.Serialize(xmlWriter, genericObject);

            return stringWriter.ToString();
        }
        public T Deserialize<T>(string serializedString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            TextReader reader = new StringReader(serializedString);

            return (T)xmlSerializer.Deserialize(reader);
        }
    }
}
