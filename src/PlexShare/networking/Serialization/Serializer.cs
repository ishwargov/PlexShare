using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Networking.Serialization
{
    public class Serializer : ISerializer
    {
        /// <summary>
        /// Given an object of a generic type, the method converts it into a serialized string in XML format and returns it
        /// </summary>
        public string Serialize<T> (T genericObject)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            // StringWriter deals with string data
            StringWriter stringWriter = new StringWriter();

            // The string data which is written by StringWriter is stored here
            XmlWriter xmlWriter = XmlWriter.Create(stringWriter);

            xmlSerializer.Serialize(xmlWriter, genericObject);

            return stringWriter.ToString();
        }

        /// <summary>
        /// Given a serialized string in XML format, the method converts it into the original object and returns it
        /// </summary>
        public T Deserialize<T> (string serializedString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            // To read the string
            StringReader stringReader = new StringReader(serializedString);

            return (T)xmlSerializer.Deserialize(stringReader);
        }
    }
}
