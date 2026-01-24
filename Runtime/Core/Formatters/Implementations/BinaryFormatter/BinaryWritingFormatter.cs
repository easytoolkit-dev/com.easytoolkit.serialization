using System;
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
            if (string.IsNullOrEmpty(name))
            {
                _writer.Write((byte)BinaryFormatterTag.MemberBegin);
            }
            else
            {
                _writer.Write((byte)BinaryFormatterTag.NamedMemberBegin);
                var bytes = Encoding.UTF8.GetBytes(name);
                WriteVarint32((uint)bytes.Length);
                _writer.Write(bytes);
            }
        }

        /// <inheritdoc />
        protected override void BeginObject()
        {
            _writer.Write((byte)BinaryFormatterTag.ObjectBegin);
            WriteVarint32((uint)_nodeDepth);
            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndObject()
        {
            _nodeDepth--;
            WriteVarint32((uint)_nodeDepth);
            _writer.Write((byte)BinaryFormatterTag.ObjectEnd);
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            _writer.Write((byte)BinaryFormatterTag.ArrayBegin);
            WriteVarint32((uint)length);
            WriteVarint32((uint)_nodeDepth);
            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndArray()
        {
            _nodeDepth--;
            WriteVarint32((uint)_nodeDepth);
            _writer.Write((byte)BinaryFormatterTag.ArrayEnd);
        }

        /// <inheritdoc />
        public override void Format(ref int value)
        {
            // Use zigzag encoding to handle negative numbers
            uint encoded = (uint)((value << 1) ^ (value >> 31));
            WriteVarint32(encoded);
        }

        /// <inheritdoc />
        public override void Format(ref sbyte value)
        {
            // Use zigzag encoding to handle negative numbers
            int encoded = (value << 1) ^ (value >> 7);
            _writer.Write((byte)encoded);
        }

        /// <inheritdoc />
        public override void Format(ref short value)
        {
            // Use zigzag encoding to handle negative numbers
            int encoded = (value << 1) ^ (value >> 15);
            WriteVarint32((uint)encoded);
        }

        /// <inheritdoc />
        public override void Format(ref long value)
        {
            // Use zigzag encoding to handle negative numbers
            ulong encoded = ((ulong)value << 1) ^ (ulong)(value >> 63);
            WriteVarint64(encoded);
        }

        /// <inheritdoc />
        public override void Format(ref byte value)
        {
            _writer.Write(value);
        }

        /// <inheritdoc />
        public override void Format(ref ushort value)
        {
            WriteVarint32(value);
        }

        /// <inheritdoc />
        public override void Format(ref uint value)
        {
            WriteVarint32(value);
        }

        /// <inheritdoc />
        public override void Format(ref ulong value)
        {
            WriteVarint64(value);
        }

        /// <inheritdoc />
        public override void Format(ref bool value)
        {
            _writer.Write(value ? (byte)1 : (byte)0);
        }

        /// <inheritdoc />
        public override void Format(ref float value)
        {
            _writer.Write(value);
        }

        /// <inheritdoc />
        public override void Format(ref double value)
        {
            _writer.Write(value);
        }

        /// <inheritdoc />
        public override void Format(ref string str)
        {
            if (str == null)
            {
                WriteVarint32(0);
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(str);
            WriteVarint32((uint)bytes.Length);
            _writer.Write(bytes);
        }

        /// <inheritdoc />
        public override void Format(ref byte[] data)
        {
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            _writer.Write(data);
        }

        /// <inheritdoc />
        public override void Format(ref UnityEngine.Object unityObject)
        {
            var index = RegisterReference(unityObject);
            WriteVarint32((uint)index);
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

        /// <summary>Writes a 64-bit unsigned integer using variable-length encoding.</summary>
        /// <param name="value">The value to write.</param>
        private void WriteVarint64(ulong value)
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
