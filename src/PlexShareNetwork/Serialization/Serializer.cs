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
        public string Serialize<T> (T genericObject)
        {
            // Storing the generic data type in string form
            string dataType = genericObject.GetType().ToString();
            Wrapper<T> wrapperObject = new Wrapper<T>(dataType, genericObject);

            XmlSerializer xmlWrapperSerializer;
            try
            {
                xmlWrapperSerializer = new XmlSerializer(typeof(Wrapper<T>));
            }
            catch(InvalidOperationException e)
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
            catch(ArgumentNullException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.Serialize() function.");
                Trace.WriteLine("[Networking] StringWriter object is null.");
                Trace.WriteLine($"{e.StackTrace}");
                return null;
            }

            // Serializing the provided object
            try
            {
                xmlWrapperSerializer.Serialize(xmlWriter, wrapperObject);
            }
            catch(InvalidOperationException e)
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
        public T Deserialize<T> (string serializedString)
        {
            T genericObject = default(T);

            // Getting the wrapper object which contains the generic object and its datatype
            Wrapper<T> wrapperObject = ReturnDeserializedWrapperObject<T>(serializedString);

            try
            {
                genericObject = wrapperObject.genericObject;
            }
            catch(NullReferenceException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.Deserialize() function.");
                Trace.WriteLine("[Networking] The wrapper object is null.");
                Trace.WriteLine($"{e.StackTrace}");
            }

            return genericObject;
        }

        /// <summary>
        /// Given a serialized string, the method returns the type of the generic object which was serialized
        /// </summary>
        public string GetObjectType<T>(string serializedString)
        {
            string dataType = null;

            // Getting the wrapper object which contains the generic object and its datatype
            Wrapper<T> wrapperObject = ReturnDeserializedWrapperObject<T>(serializedString);

            try
            {
                dataType = wrapperObject.dataType;
            }
            catch (NullReferenceException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.Deserialize() function.");
                Trace.WriteLine("[Networking] The wrapper object is null.");
                Trace.WriteLine($"{e.StackTrace}");
            }

            return dataType;
        }

        /// <summary>
        /// Given a serialized string in XML format, the method converts it into the wrapper object and returns it
        /// </summary>
        private Wrapper<T> ReturnDeserializedWrapperObject<T>(string serializedString)
        {
            XmlSerializer xmlWrapperSerializer;
            try
            {
                xmlWrapperSerializer = new XmlSerializer(typeof(Wrapper<T>));
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.ReturnDeserializedWrapperObject() function.");
                Trace.WriteLine("[Networking] Could not create a Serializer for the given type.");
                Trace.WriteLine($"{e.StackTrace}");
                return default(Wrapper<T>);
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
                return default(Wrapper<T>);
            }

            // The object to be returned
            Wrapper<T> wrapperObject = null;
            try
            {
                wrapperObject = (Wrapper<T>)xmlWrapperSerializer.Deserialize(stringReader);
            }
            catch (ArgumentNullException e)
            {
                Trace.WriteLine("[Networking] Exception caught in Serializer.ReturnDeserializedWrapperObject() function.");
                Trace.WriteLine("[Networking] Could not deserialize the object of the given type.");
                Trace.WriteLine($"{e.StackTrace}");
                return null;
            }

            // Returning the wrapper object
            return wrapperObject;
        }
    }

    // Used to store the datatype of the object to be serialized
    public class Wrapper<T>
    {
        public string dataType;
        public T genericObject;

        // Empty constructor
        public Wrapper() {}

        public Wrapper(string dataType, T genericObject)
        {
            this.dataType = dataType;
            this.genericObject = genericObject;
        }
    }
}
