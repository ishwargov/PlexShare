/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the definition of the ISerializer interface. It contains method blueprints to serialize and deserialize data
/// </summary>

namespace PlexShareNetwork.Serialization
{
    public interface ISerializer
    {
        /// <summary>
        /// Given an object of a generic type, the method converts it into a serialized string and returns it
        /// </summary>
        public string Serialize<T> (T genericObject);

        /// <summary>
        /// Given a serialized string, the method converts it into the original object and returns it
        /// </summary>
        public T Deserialize<T> (string serializedString);

        /// <summary>
        /// Given a serialized string, the method returns the type of the generic object which was serialized
        /// </summary>
        public string GetObjectType<T>(string serializedString);
    }
}
