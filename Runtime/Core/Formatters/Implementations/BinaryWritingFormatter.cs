using System.IO;
using System.Text;
using EasyToolKit.Core.Textual;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Binary writing formatter implementation. Serializes data to a binary format
    /// using length-prefixed field names and varint encoding.
    /// </summary>
    public sealed class BinaryWritingFormatter : WritingFormatterBase
    {
        private readonly BinaryWriter _writer;
        private int _nodeDepth;

        /// <summary>Creates a new binary writing formatter.</summary>
        /// <param name="stream">The stream to write to.</param>
        public BinaryWritingFormatter(Stream stream)
        {
            _writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
            _nodeDepth = 0;
        }

        /// <inheritdoc />
        public override SerializationFormat Type => SerializationFormat.Binary;

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
            _nodeDepth++;
        }

        /// <inheritdoc />
        public override void EndObject()
        {
            _nodeDepth--;
        }

        /// <inheritdoc />
        public override bool Format(ref int value)
        {
            _writer.Write(value);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref Varint32 value)
        {
            WriteVarint32(value.Value);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref SizeTag size)
        {
            WriteVarint32(size.Size);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref bool value)
        {
            _writer.Write(value ? (byte)1 : (byte)0);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref float value)
        {
            _writer.Write(value);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref double value)
        {
            _writer.Write(value);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref string str)
        {
            if (str == null)
            {
                WriteVarint32(0);
                return true;
            }

            var bytes = Encoding.UTF8.GetBytes(str);
            WriteVarint32((uint)bytes.Length);
            _writer.Write(bytes);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref byte[] data)
        {
            if (data == null)
            {
                WriteVarint32(0);
                return true;
            }

            WriteVarint32((uint)data.Length);
            _writer.Write(data);
            return true;
        }

        /// <inheritdoc />
        public override bool Format(ref UnityEngine.Object unityObject)
        {
            var index = RegisterReference(unityObject);
            WriteVarint32((uint)index);
            return true;
        }

        /// <summary>Writes a 32-bit unsigned integer using variable-length encoding.</summary>
        /// <param name="value">The value to write.</param>
        private void WriteVarint32(uint value)
        {
            while (value > 0x7F)
            {
                _writer.Write((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
            _writer.Write((byte)value);
        }
    }
}
