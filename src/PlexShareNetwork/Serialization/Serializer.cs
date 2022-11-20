/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains an implementation of the ISerializer interface
/// </summary>

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PlexShareNetwork.Serialization
{
    public class Serializer : ISerializer
    {
        /// <summary>
        /// Given an object of a generic type, the method converts it into a serialized string in XML format and returns it
        /// </summary>
        /// <param name="genericObject">
        /// The object to be serialized
        /// </param>
        /// <returns>
        /// The serialized string of the generic object
        /// </returns>
        public string Serialize<T> (T genericObject)
        {
            if (genericObject == null)
            {
                Trace.WriteLine("[Networking] Could not serialize a null object.");
                return null;
            }

            XmlSerializer xmlSerializer;
            try
            {
                xmlSerializer = new XmlSerializer(typeof(T));
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.Serialize() function.");
                Trace.WriteLine("[Networking] Could not create a Serializer for the given type.");
                Trace.WriteLine($"{e.StackTrace}");
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
            catch (ArgumentNullException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.Serialize() function.");
                Trace.WriteLine("[Networking] StringWriter object is null.");
                Trace.WriteLine($"{e.StackTrace}");
                return null;
            }

            // Serializing the provided object
            try
            {
                xmlSerializer.Serialize(xmlWriter, genericObject);
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.Serialize() function.");
                Trace.WriteLine("[Networking] Could not serialize the object of the given type.");
                Trace.WriteLine($"{e.StackTrace}");
                return null;
            }

            return stringWriter.ToString();
        }

        /// <summary>
        /// Given a serialized string in XML format, the method converts it into the original object and returns it
        /// </summary>
        /// <param name="serializedString">
        /// The string to be deserialized
        /// </param>
        /// <returns>
        /// The original object after deserializing the string
        /// </returns>
        public T Deserialize<T> (string serializedString)
        {
            XmlSerializer xmlSerializer;
            try
            {
                xmlSerializer = new XmlSerializer(typeof(T));
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.ReturnDeserializedWrapperObject() function.");
                Trace.WriteLine("[Networking] Could not create a Serializer for the given type.");
                Trace.WriteLine($"{e.StackTrace}");
                return default(T);
            }

            // To read the string
            StringReader stringReader;
            try
            {
                stringReader = new StringReader(serializedString);
            }
            catch (ArgumentNullException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.ReturnDeserializedWrapperObject() function.");
                Trace.WriteLine("[Networking] The serialized string is null.");
                Trace.WriteLine($"{e.StackTrace}");
                return default(T);
            }

            // The object to be returned
            T genericObject = default(T);
            try
            {
                genericObject = (T)xmlSerializer.Deserialize(stringReader);
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.ReturnDeserializedWrapperObject() function.");
                Trace.WriteLine("[Networking] Could not deserialize the object of the given type.");
                Trace.WriteLine($"{e.StackTrace}");
                return default(T);
            }

            // Returning the wrapper object
            return genericObject;
        }
    }
}
