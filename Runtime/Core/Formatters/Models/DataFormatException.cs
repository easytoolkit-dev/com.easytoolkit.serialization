using System;

namespace EasyToolkit.Serialization.Formatters
{
    /// <summary>
    /// Exception thrown when data format validation fails during serialization or deserialization.
    /// </summary>
    public class DataFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormatException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DataFormatException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormatException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DataFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
