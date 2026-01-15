namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Reading/deserialization formatter interface.
    /// Combines core formatter with object reference reading capabilities.
    /// </summary>
    public interface IReadingFormatter : IDataFormatter, IObjectReferenceReader
    {
        /// <summary>Always returns Input direction for reading formatters.</summary>
        new FormatterDirection Direction { get; }
    }
}
