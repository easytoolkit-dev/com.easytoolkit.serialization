using System.IO;

namespace EasyToolKit.Serialization
{
    public interface IFormatterFactory
    {
        /// <summary>
        /// Creates a reading formatter for deserialization from the specified stream.
        /// </summary>
        IReadingFormatter CreateReader(SerializationFormat type, Stream input);

        /// <summary>
        /// Creates a writing formatter for serialization to the specified stream.
        /// </summary>
        IWritingFormatter CreateWriter(SerializationFormat type, Stream output);
    }
}
