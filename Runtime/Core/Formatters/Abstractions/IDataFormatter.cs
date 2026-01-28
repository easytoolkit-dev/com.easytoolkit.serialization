using System;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Formatters
{
    /// <summary>
    /// Defines a contract for serializing and deserializing data to different formats.
    /// Formatters handle low-level data encoding/decoding for primitive types, objects,
    /// arrays, and member boundaries during the serialization process.
    /// </summary>
    /// <remarks>
    /// This interface provides format-agnostic serialization capabilities. Concrete implementations
    /// support specific serialization formats such as Binary, JSON, XML, or YAML. The interface
    /// uses ref parameters to support both reading (deserialization) and writing (serialization)
    /// operations through the same method signatures.
    ///
    /// The serialization model follows a hierarchical structure:
    /// - Objects (complex types) contain members
    /// - Members can contain primitive values, objects, or arrays
    /// - Arrays contain sequences of values
    ///
    /// For format-specific implementations, see <see cref="IWritingFormatter"/> and
    /// <see cref="IReadingFormatter"/> interfaces.
    /// </remarks>
    public interface IDataFormatter : IDisposable
    {
        /// <summary>
        /// Gets the serialization format type supported by this formatter.
        /// </summary>
        SerializationFormat FormatType { get; }

        /// <summary>
        /// Gets or sets the formatter-specific settings for this formatter.
        /// </summary>
        DataFormatterSettings Settings { get; set; }

        /// <summary>
        /// Gets the operation type (Read or Write) performed by this formatter.
        /// </summary>
        FormatterOperation Operation { get; }

        /// <summary>
        /// Begins serialization of a member with an optional name.
        /// </summary>
        /// <param name="name">The member name, or null/empty for unnamed members.</param>
        /// <remarks>
        /// This method is only effective within an Object scope (after calling <see cref="BeginObject"/>).
        /// When called outside of an Object scope, this operation is skipped by the formatter.
        /// </remarks>
        void BeginMember(string name);

        /// <summary>
        /// Begins serialization of a complex object with optional type information.
        /// </summary>
        /// <param name="type">The type of the object, or null if type information is not needed.</param>
        /// <remarks>
        /// Objects are complex types containing multiple members.
        /// The type parameter is used by formatters that support polymorphic serialization
        /// (e.g., Binary formatter can write type information when IncludeObjectType is enabled).
        /// Call <see cref="EndObject"/> to complete the object.
        /// </remarks>
        void BeginObject([CanBeNull] Type type = null);

        /// <summary>
        /// Ends the current object serialization scope.
        /// </summary>
        /// <remarks>
        /// Must be called after <see cref="BeginObject"/> and all member serialization.
        /// </remarks>
        void EndObject();

        /// <summary>
        /// Begins serialization of an array with a specified length.
        /// </summary>
        /// <param name="length">The array length to read or write.</param>
        /// <remarks>
        /// Arrays are sequences of values. The length parameter allows the formatter
        /// to serialize or deserialize the array size information.
        /// - During serialization: length is written to the output
        /// - During deserialization: length is read from the input
        /// Call <see cref="EndArray"/> to complete the array.
        /// </remarks>
        void BeginArray(ref int length);

        /// <summary>
        /// Ends the current array serialization scope.
        /// </summary>
        /// <remarks>
        /// Must be called after <see cref="BeginArray"/> and all element serialization.
        /// </remarks>
        void EndArray();

        /// <summary>
        /// Serializes or deserializes a 32-bit signed integer value.
        /// </summary>
        /// <param name="value">The integer value to read or write.</param>
        void Format(ref int value);

        /// <summary>
        /// Serializes or deserializes an 8-bit signed integer value.
        /// </summary>
        /// <param name="value">The signed byte value to read or write.</param>
        void Format(ref sbyte value);

        /// <summary>
        /// Serializes or deserializes a 16-bit signed integer value.
        /// </summary>
        /// <param name="value">The short integer value to read or write.</param>
        void Format(ref short value);

        /// <summary>
        /// Serializes or deserializes a 64-bit signed integer value.
        /// </summary>
        /// <param name="value">The long integer value to read or write.</param>
        void Format(ref long value);

        /// <summary>
        /// Serializes or deserializes an 8-bit unsigned integer value.
        /// </summary>
        /// <param name="value">The byte value to read or write.</param>
        void Format(ref byte value);

        /// <summary>
        /// Serializes or deserializes a 16-bit unsigned integer value.
        /// </summary>
        /// <param name="value">The unsigned short integer value to read or write.</param>
        void Format(ref ushort value);

        /// <summary>
        /// Serializes or deserializes a 32-bit unsigned integer value.
        /// </summary>
        /// <param name="value">The unsigned integer value to read or write.</param>
        void Format(ref uint value);

        /// <summary>
        /// Serializes or deserializes a 64-bit unsigned integer value.
        /// </summary>
        /// <param name="value">The unsigned long integer value to read or write.</param>
        void Format(ref ulong value);

        /// <summary>
        /// Serializes or deserializes a boolean value.
        /// </summary>
        /// <param name="value">The boolean value to read or write.</param>
        void Format(ref bool value);

        /// <summary>
        /// Serializes or deserializes a single-precision floating-point value.
        /// </summary>
        /// <param name="value">The float value to read or write.</param>
        void Format(ref float value);

        /// <summary>
        /// Serializes or deserializes a double-precision floating-point value.
        /// </summary>
        /// <param name="value">The double value to read or write.</param>
        void Format(ref double value);

        /// <summary>
        /// Serializes or deserializes a string value.
        /// </summary>
        /// <param name="str">The string value to read or write.</param>
        /// <remarks>
        /// Null strings are supported. Format-specific encoding (UTF-8, UTF-16, etc.)
        /// is handled by the concrete formatter implementation.
        /// </remarks>
        void Format(ref string str);

        /// <summary>
        /// Serializes or deserializes a byte array.
        /// </summary>
        /// <param name="data">The byte array to read or write.</param>
        /// <remarks>
        /// Null arrays are supported. The format determines how array length and
        /// content are encoded.
        /// </remarks>
        void Format(ref byte[] data);

        /// <summary>
        /// Serializes or deserializes an sbyte array.
        /// </summary>
        /// <param name="data">The sbyte array to read or write.</param>
        /// <remarks>
        /// Null arrays are supported. The format determines how array length and
        /// content are encoded.
        /// </remarks>
        void Format(ref sbyte[] data);

        /// <summary>
        /// Serializes or deserializes a short array.
        /// </summary>
        /// <param name="data">The short array to read or write.</param>
        /// <remarks>
        /// Null arrays are supported. The format determines how array length and
        /// content are encoded.
        /// </remarks>
        void Format(ref short[] data);

        /// <summary>
        /// Serializes or deserializes an int array.
        /// </summary>
        /// <param name="data">The int array to read or write.</param>
        /// <remarks>
        /// Null arrays are supported. The format determines how array length and
        /// content are encoded.
        /// </remarks>
        void Format(ref int[] data);

        /// <summary>
        /// Serializes or deserializes a long array.
        /// </summary>
        /// <param name="data">The long array to read or write.</param>
        /// <remarks>
        /// Null arrays are supported. The format determines how array length and
        /// content are encoded.
        /// </remarks>
        void Format(ref long[] data);

        /// <summary>
        /// Serializes or deserializes a ushort array.
        /// </summary>
        /// <param name="data">The ushort array to read or write.</param>
        /// <remarks>
        /// Null arrays are supported. The format determines how array length and
        /// content are encoded.
        /// </remarks>
        void Format(ref ushort[] data);

        /// <summary>
        /// Serializes or deserializes a uint array.
        /// </summary>
        /// <param name="data">The uint array to read or write.</param>
        /// <remarks>
        /// Null arrays are supported. The format determines how array length and
        /// content are encoded.
        /// </remarks>
        void Format(ref uint[] data);

        /// <summary>
        /// Serializes or deserializes a ulong array.
        /// </summary>
        /// <param name="data">The ulong array to read or write.</param>
        /// <remarks>
        /// Null arrays are supported. The format determines how array length and
        /// content are encoded.
        /// </remarks>
        void Format(ref ulong[] data);

        /// <summary>
        /// Serializes or deserializes a Unity object reference.
        /// </summary>
        /// <param name="unityObject">The Unity object reference to read or write.</param>
        /// <remarks>
        /// Unity objects are handled through reference tracking. During writing,
        /// objects are registered and assigned an index. During reading, the index
        /// is resolved back to the object reference via an object table.
        /// </remarks>
        void Format(ref UnityEngine.Object unityObject);

        /// <summary>
        /// Serializes or deserializes a generic primitive value using direct memory operations.
        /// </summary>
        /// <typeparam name="T">The generic primitive type to format.</typeparam>
        /// <param name="value">The generic primitive value to read or write.</param>
        /// <remarks>
        /// This method provides optimized serialization for generic primitive types (primitives, enums, structs)
        /// using direct memory copy. This is only supported in Binary format mode for high-performance
        /// serialization without per-type encoding logic. This is particularly useful for custom structs and enums.
        ///
        /// For other serialization formats (JSON, XML, etc.), use the typed Format methods
        /// (e.g., <see cref="Format(ref int)"/>, <see cref="Format(ref string)"/>).
        ///
        /// Calling this method on non-Binary formatters will throw a <see cref="NotSupportedException"/>.
        /// </remarks>
        void FormatGenericPrimitive<T>(ref T value) where T : unmanaged;

        /// <summary>
        /// Serializes or deserializes an array of generic primitive values using direct memory operations.
        /// </summary>
        /// <typeparam name="T">The generic primitive element type to format.</typeparam>
        /// <param name="data">The array of generic primitive values to read or write.</param>
        /// <remarks>
        /// This method provides optimized serialization for arrays of generic primitive types (primitives, enums, structs)
        /// using direct memory copy. This is only supported in Binary format mode for high-performance
        /// serialization without per-element encoding logic. This is particularly useful for arrays of custom structs and enums.
        ///
        /// For other serialization formats (JSON, XML, etc.), use the typed Format methods
        /// (e.g., <see cref="Format(ref int[])"/>, <see cref="Format(ref long[])"/>).
        ///
        /// Calling this method on non-Binary formatters will throw a <see cref="NotSupportedException"/>.
        /// Null arrays are supported and will be serialized as a zero-length array.
        /// </remarks>
        void FormatGenericPrimitive<T>(ref T[] data) where T : unmanaged;
    }
}
