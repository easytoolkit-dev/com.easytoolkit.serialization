using System.IO;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Factory interface for creating formatter instances.
    /// Registered with DI container for dependency injection support.
    /// </summary>
    public interface IFormatterFactory
    {
        /// <summary>
        /// Creates a reading formatter for deserialization from the specified stream.
        /// </summary>
        IReadingFormatter CreateReader(FormatterType type, Stream input);

        /// <summary>
        /// Creates a writing formatter for serialization to the specified stream.
        /// </summary>
        IWritingFormatter CreateWriter(FormatterType type, Stream output);
    }
}
