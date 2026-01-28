using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization
{
    /// <summary>
    /// Provides configuration settings for serialization operations.
    /// </summary>
    public class SerializationSettings
    {
        /// <summary>
        /// Gets the default singleton instance of serialization settings.
        /// </summary>
        public static SerializationSettings Default { get; } = new();

        /// <summary>
        /// Gets or sets the formatter settings for the binary serialization format.
        /// </summary>
        public BinaryFormatterSettings BinaryFormatterSettings { get; set; } = new();
    }
}
