using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Provides extension methods for <see cref="IDataFormatter"/> to create scope-based serialization operations.
    /// </summary>
    public static class DataFormatterExtensions
    {
        /// <summary>
        /// Creates a member scope that automatically manages the BeginMember/EndMember calls.
        /// </summary>
        /// <param name="formatter">The data formatter to create the scope for.</param>
        /// <param name="name">The member name, or null for unnamed members.</param>
        /// <returns>A <see cref="FormatterMemberScope"/> that will automatically end the member when disposed.</returns>
        public static FormatterMemberScope EnterMember(this IDataFormatter formatter, string name)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            return FormatterMemberScope.Create(formatter, name);
        }

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
        /// Creates an array scope that automatically manages the BeginArray/EndArray calls.
        /// </summary>
        /// <param name="formatter">The data formatter to create the scope for.</param>
        /// <returns>A <see cref="FormatterArrayScope"/> that will automatically end the array when disposed.</returns>
        public static FormatterArrayScope EnterArray(this IDataFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            return FormatterArrayScope.Create(formatter);
        }
    }
}
