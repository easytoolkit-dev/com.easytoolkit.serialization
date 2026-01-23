using System;

namespace EasyToolKit.Serialization
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
        SerializationFormat Type { get; }

        /// <summary>
        /// Gets the operation type (Read or Write) performed by this formatter.
        /// </summary>
        FormatterOperation Operation { get; }

        /// <summary>
        /// Begins serialization of a member with an optional name.
        /// </summary>
        /// <param name="name">The member name, or null/empty for unnamed members.</param>
        /// <remarks>
        /// Members represent fields or properties within an object. The name can be used
        /// for named serialization formats (JSON, XML) or omitted for compact binary formats.
        /// Must be followed by a call to <see cref="EndMember"/> after writing the member value.
        /// </remarks>
        void BeginMember(string name);

        /// <summary>
        /// Ends the current member serialization scope.
        /// </summary>
        /// <remarks>
        /// Must be called after <see cref="BeginMember"/> and writing the member value.
        /// </remarks>
        void EndMember();

        /// <summary>
        /// Begins serialization of a complex object.
        /// </summary>
        /// <remarks>
        /// Objects are complex types containing multiple members. After calling this method,
        /// use <see cref="BeginMember"/> and <see cref="EndMember"/> pairs to serialize
        /// each member, then call <see cref="EndObject"/> to complete the object.
        /// </remarks>
        void BeginObject();

        /// <summary>
        /// Ends the current object serialization scope.
        /// </summary>
        /// <remarks>
        /// Must be called after <see cref="BeginObject"/> and all member serialization.
        /// </remarks>
        void EndObject();

        /// <summary>
        /// Begins serialization of an array.
        /// </summary>
        /// <remarks>
        /// Arrays are sequences of values. After calling this method, serialize each
        /// element sequentially, then call <see cref="EndArray"/> to complete the array.
        /// </remarks>
        void BeginArray();

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
        /// Serializes or deserializes a Unity object reference.
        /// </summary>
        /// <param name="unityObject">The Unity object reference to read or write.</param>
        /// <remarks>
        /// Unity objects are handled through reference tracking. During writing,
        /// objects are registered and assigned an index. During reading, the index
        /// is resolved back to the object reference via an object table.
        /// </remarks>
        void Format(ref UnityEngine.Object unityObject);
    }
}
