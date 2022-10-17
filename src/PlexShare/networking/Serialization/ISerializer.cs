using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.Serialization
{
    interface ISerializer
    {
        /// <summary>
        /// Given an object of a generic type, the method converts it into a serialized string and returns it
        /// </summary>
        public string Serialize<T> (T genericObject);

        /// <summary>
        /// Given a serialized string, the method converts it into the original object and returns it
        /// </summary>
        public T Deserialize<T> (string serializedString);
    }
}
