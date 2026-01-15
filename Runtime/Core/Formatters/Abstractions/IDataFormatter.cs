using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Core formatter interface for serialization and deserialization operations.
    /// High-level modules depend on this abstraction for dependency inversion.
    /// </summary>
    public interface IDataFormatter
    {
        /// <summary>Gets the format type of this formatter.</summary>
        FormatterType Type { get; }

        /// <summary>Gets the operation direction (input or output).</summary>
        FormatterDirection Direction { get; }

        /// <summary>Begins a new member/field with the specified name.</summary>
        void BeginMember(string name);

        /// <summary>Ends the current member/field.</summary>
        void EndMember();

        /// <summary>Begins a new object/node context.</summary>
        void BeginObject();

        /// <summary>Ends the current object/node context.</summary>
        void EndObject();

        /// <summary>Formats an integer value.</summary>
        bool Format(ref int value);

        /// <summary>Formats a variable-length integer value.</summary>
        bool Format(ref Varint32 value);

        /// <summary>Formats a collection size tag.</summary>
        bool Format(ref SizeTag size);

        /// <summary>Formats a boolean value.</summary>
        bool Format(ref bool value);

        /// <summary>Formats a float value.</summary>
        bool Format(ref float value);

        /// <summary>Formats a double value.</summary>
        bool Format(ref double value);

        /// <summary>Formats a string value.</summary>
        bool Format(ref string str);

        /// <summary>Formats a byte array.</summary>
        bool Format(ref byte[] data);

        /// <summary>Formats a Unity object reference.</summary>
        bool Format(ref UnityEngine.Object unityObject);
    }
}
