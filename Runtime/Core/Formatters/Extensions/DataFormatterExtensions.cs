using System;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Formatters
{
    /// <summary>
    /// Provides extension methods for <see cref="IDataFormatter"/> to create scope-based serialization operations.
    /// </summary>
    public static class DataFormatterExtensions
    {
        /// <summary>
        /// Creates an object scope that automatically manages the BeginObject/EndObject calls.
        /// </summary>
        /// <param name="formatter">The data formatter to create the scope for.</param>
        /// <returns>A <see cref="FormatterObjectScope"/> that will automatically end the object when disposed.</returns>
        public static FormatterObjectScope EnterObject(this IDataFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            return FormatterObjectScope.Create(formatter);
        }

        /// <summary>
        /// Creates an object scope that automatically manages the BeginObject/EndObject calls.
        /// </summary>
        /// <param name="formatter">The data formatter to create the scope for.</param>
        /// <param name="type">
        /// The type of the object. In Write mode, this type is written to the output.
        /// In Read mode, validates that the type in the data matches the expected type.
        /// </param>
        /// <returns>A <see cref="FormatterObjectScope"/> that will automatically end the object when disposed.</returns>
        public static FormatterObjectScope EnterObject(this IDataFormatter formatter, Type type)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            return FormatterObjectScope.Create(formatter, type);
        }

        /// <summary>
        /// Creates an array scope that automatically manages the BeginArray/EndArray calls.
        /// </summary>
        /// <param name="formatter">The data formatter to create the scope for.</param>
        /// <param name="length">The array length to read or write.</param>
        /// <returns>A <see cref="FormatterArrayScope"/> that will automatically end the array when disposed.</returns>
        public static FormatterArrayScope EnterArray(this IDataFormatter formatter, ref int length)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            return FormatterArrayScope.Create(formatter, ref length);
        }
    }
}
