/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains an implementation of the ISerializer interface.
/// </summary>

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
            XmlSerializer xmlSerializer;
            try
            {
                xmlSerializer = new XmlSerializer(typeof(T));
            }
            catch(InvalidOperationException e)
            {
                // Arises if the class 'T' does not have an empty constructor
                Console.WriteLine(e.StackTrace);
                return null;
            }

            // StringWriter deals with string data
            StringWriter stringWriter = new StringWriter();

            // The string data which is written by StringWriter is stored here
            XmlWriter xmlWriter = null;
            try
            {
                xmlWriter = XmlWriter.Create(stringWriter);
            }
            catch(ArgumentNullException e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }

            // Serializing the provided object
            try
            {
                xmlSerializer.Serialize(xmlWriter, genericObject);
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }

            return stringWriter.ToString();
        }

        /// <summary>
        /// Given a serialized string in XML format, the method converts it into the original object and returns it
        /// </summary>
        public T Deserialize<T> (string serializedString)
        {
            XmlSerializer xmlSerializer;
            try
            {
                xmlSerializer = new XmlSerializer(typeof(T));
            }
            catch (InvalidOperationException e)
            {
                // Arises if the class 'T' does not have an empty constructor
                Console.WriteLine(e.StackTrace);
                return default(T);
            }

            // To read the string
            StringReader stringReader;
            try
            {
                stringReader = new StringReader(serializedString);
            }
            catch(ArgumentNullException e)
            {
                Console.WriteLine(e.StackTrace);
                return default(T);
            }

            // The object to be returned
            T returnObject = default(T);
            try
            {
                returnObject = (T)xmlSerializer.Deserialize(stringReader);
            }
            catch(ArgumentNullException e)
            {
                Console.WriteLine(e.StackTrace);
                return default(T);
            }

            return returnObject;
        }
    }
}
