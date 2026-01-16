using System.IO;
using System.Text;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Binary reading formatter implementation. Deserializes data from a binary format
    /// using length-prefixed field names and varint encoding.
    /// </summary>
    public sealed class BinaryReadingFormatter : ReadingFormatterBase
    {
        private readonly BinaryReader _reader;

        /// <summary>Creates a new binary reading formatter.</summary>
        /// <param name="stream">The stream to read from.</param>
        public BinaryReadingFormatter(Stream stream)
        {
            _reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        }

        /// <inheritdoc />
        public override FormatterType Type => FormatterType.Binary;

        /// <inheritdoc />
        public override void BeginMember(string name)
        {
            // No-op for binary format
        }

        /// <inheritdoc />
        public override void EndMember()
        {
            // No-op for binary format
        }

        /// <inheritdoc />
        public override void BeginObject()
        {
            // No-op for binary format
        }

        /// <inheritdoc />
        public override void EndObject()
        {
            // No-op for binary format
        }

        /// <inheritdoc />
        public override bool Format(ref int value)
        {
            value = _reader.ReadInt32();
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref Varint32 value)
        {
            value = new Varint32(ReadVarint32());
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref SizeTag size)
        {
            size = new SizeTag(ReadVarint32());
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref bool value)
        {
            var byteValue = _reader.ReadByte();
            value = byteValue != 0;
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref float value)
        {
            value = _reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref double value)
        {
            value = _reader.ReadDouble();
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref string str)
        {
            var length = ReadVarint32();
            if (length == 0)
            {
                str = null;
                return true;
            }

            var bytes = _reader.ReadBytes((int)length);
            str = Encoding.UTF8.GetString(bytes);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref byte[] data)
        {
            var length = ReadVarint32();
            if (length == 0)
            {
                data = null;
                return true;
            }

            data = _reader.ReadBytes((int)length);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref UnityEngine.Object unityObject)
        {
            var index = ReadVarint32();
            unityObject = ResolveReference((int)index);
            return true;
        }

        /// <summary>Reads a 32-bit unsigned integer using variable-length decoding.</summary>
        /// <returns>The decoded value.</returns>
        private uint ReadVarint32()
        {
            uint value = 0;
            int shift = 0;
            byte b;
            do
            {
                b = _reader.ReadByte();
                value |= (uint)(b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return value;
        }
    }
}
