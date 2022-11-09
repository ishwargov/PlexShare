/******************************************************************************
 * Filename    = IContentSerializer.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Serialization Interface for Content module
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareContent
{
    public interface IContentSerializer
    {
        /// <summary>
        ///     Serializes an object into XML strings.
        /// </summary>
        /// <typeparam name="T">Object Type.</typeparam>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>Serialized XML string.</returns>
        string Serialize<T>(T objectToSerialize) where T : new();

        /// <summary>
        ///     Returns the type of object serialized as the XML string.
        /// </summary>
        /// <param name="serializedString">Serialized XML string.</param>
        /// <param name="nameSpace">Namespace of the module deserializing the string.</param>
        /// <returns>Object Type.</returns>
        string GetObjectType(string serializedString, string nameSpace);

        /// <summary>
        ///     Deserializes the XML string into the corresponding object.
        /// </summary>
        /// <typeparam name="T">Type of object being deserialized.</typeparam>
        /// <param name="serializedString">Serialized XML string.</param>
        /// <returns>Deserialized Object.</returns>
        T Deserialize<T>(string serializedString);
    }
}
