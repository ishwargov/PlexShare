/*
 * Name : Saurabh Kumar
 * Roll : 111901046
 * FileName: IDashboardSerializer
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard
{
    public interface IDashboardSerializer
    {
        /// <summary>
        ///     Serializes an object into XML strings.
        /// </summary>
        /// <typeparam name="T">Object Type.</typeparam>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>Serialized XML string.</returns>
        string Serialize<T>(T objectToSerialize) where T : new();

       
        /// <summary>
        ///     Deserializes the XML string into the corresponding object.
        /// </summary>
        /// <typeparam name="T">Type of object being deserialized.</typeparam>
        /// <param name="serializedString">Serialized XML string.</param>
        /// <returns>Deserialized Object.</returns>
        T Deserialize<T>(string serializedString);
    }
}
