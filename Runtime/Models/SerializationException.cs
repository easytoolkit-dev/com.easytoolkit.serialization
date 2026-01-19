using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Exception thrown when serialization or deserialization fails.
    /// </summary>
    public class SerializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public SerializationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public SerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
