/******************************************************************************
 * Filename    = ContentSerializer.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class Handling Serialization for Content module
 *****************************************************************************/

using System;
using System.Diagnostics;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace PlexShareContent
{
    /// <summary>
    ///     Wrapper object to store serilized object's type and serilized string representation.
    /// </summary>
    public class MetaObject
    {
        public string data;
        public string typ;

        public MetaObject()
        {
        }

        public MetaObject(string typ, string data)
        {
            this.data = data;
            this.typ = typ;
        }
    }
    public class ContentSerializer : IContentSerializer
    {
            private readonly JsonSerializerSettings _jsonSerializerSettings;

            public ContentSerializer()
            {
                _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            }

            /// <inheritdoc />
            string IContentSerializer.Serialize<T>(T objectToSerialize)
            {
                try
                {
                    var json = SerializeJson(objectToSerialize);
                    var obj = new MetaObject(typeof(T).ToString(), json);
                    return SerializeJson(obj);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"[Networking] Error while serializing: {ex.Message}");
                    throw;
                }
            }

            /// <inheritdoc />
            string IContentSerializer.GetObjectType(string serializedString, string nameSpace)
            {
                // json string
                var obj = DeserializeJson<MetaObject>(serializedString);
                return obj.typ;
            }

            /// <inheritdoc />
            T IContentSerializer.Deserialize<T>(string serializedString)
            {
                try
                {
                    var obj = DeserializeJson<MetaObject>(serializedString);
                    return DeserializeJson<T>(obj.data);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"[Networking] Error while deserializing: {ex.Message}");
                    throw;
                }
            }

            /// <summary>
            ///     JSON supported serialization
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="objectToSerialize"></param>
            /// <returns></returns>
            private string SerializeJson<T>(T objectToSerialize)
            {
                return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, _jsonSerializerSettings);
            }

            /// <summary>
            ///     JSON supoorted deserialization.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="json"></param>
            /// <returns></returns>
            private T DeserializeJson<T>(string json)
            {
                return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
            }
        }
}
