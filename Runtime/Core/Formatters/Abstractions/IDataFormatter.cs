using System;

namespace EasyToolKit.Serialization
{
    public interface IDataFormatter
    {
        SerializationFormat Type { get; }

        FormatterOperation Operation { get; }

        /// <summary>Begins a new member/field with the specified name.</summary>
        void BeginMember(string name);

        /// <summary>Ends the current member/field.</summary>
        void EndMember();

        /// <summary>Begins a new object/node context.</summary>
        void BeginObject();

        /// <summary>Ends the current object/node context.</summary>
        void EndObject();

        /// <summary>Formats an integer value.</summary>
        void Format(ref int value);

        /// <summary>Formats a variable-length integer value.</summary>
        void Format(ref Varint32 value);

        /// <summary>Formats a collection size tag.</summary>
        void Format(ref SizeTag size);

        /// <summary>Formats a boolean value.</summary>
        void Format(ref bool value);

        /// <summary>Formats a float value.</summary>
        void Format(ref float value);

        /// <summary>Formats a double value.</summary>
        void Format(ref double value);

        /// <summary>Formats a string value.</summary>
        void Format(ref string str);

        /// <summary>Formats a byte array.</summary>
        void Format(ref byte[] data);

        /// <summary>Formats a Unity object reference.</summary>
        void Format(ref UnityEngine.Object unityObject);
    }
}
