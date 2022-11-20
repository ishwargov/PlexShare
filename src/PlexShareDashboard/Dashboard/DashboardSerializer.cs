/***********************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name: DashboardSerializer.cs
 * This file contains the implemetation of Json Serializer
 **********************************************/
using Newtonsoft.Json;
using System;

namespace PlexShareDashboard.Dashboard
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

    public class DashboardSerializer : IDashboardSerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public DashboardSerializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        /// <inheritdoc />
        string IDashboardSerializer.Serialize<T>(T objectToSerialize)
        {
            try
            {
                var json = SerializeJson(objectToSerialize);
                var obj = new MetaObject(typeof(T).ToString(), json);
                return SerializeJson(obj);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <inheritdoc />
        T IDashboardSerializer.Deserialize<T>(string serializedString)
        {
            try
            {
                var obj = DeserializeJson<MetaObject>(serializedString);
                return DeserializeJson<T>(obj.data);
            }
            catch (Exception ex)
            {
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