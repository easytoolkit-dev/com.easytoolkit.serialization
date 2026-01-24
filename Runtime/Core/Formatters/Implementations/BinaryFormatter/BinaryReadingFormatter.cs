using System;
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
        private int _nodeDepth;

        public BinaryReadingFormatter(Stream stream)
        {
            _reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
            _nodeDepth = 0;
        }

        /// <inheritdoc />
        public override SerializationFormat Type => SerializationFormat.Binary;

        /// <inheritdoc />
        public override void BeginMember(string name)
        {
            var tag = (BinaryFormatterTag)_reader.ReadByte();
            if (tag == BinaryFormatterTag.NamedMemberBegin)
            {
                var length = ReadVarint32();
                if (length > 0)
                {
                    var bytes = _reader.ReadBytes((int)length);
                    var readName = Encoding.UTF8.GetString(bytes);
                    // Verify name matches if provided
                    if (!string.IsNullOrEmpty(name) && readName != name)
                    {
                        throw new InvalidOperationException(
                            $"Member name mismatch. Expected '{name}', found '{readName}'.");
                    }
                }
            }
            else if (tag != BinaryFormatterTag.MemberBegin)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at BeginMember. Expected {BinaryFormatterTag.MemberBegin} or {BinaryFormatterTag.NamedMemberBegin}, found {tag}.");
            }
        }

        /// <inheritdoc />
        protected override void BeginObject()
        {
            var tag = (BinaryFormatterTag)_reader.ReadByte();
            if (tag != BinaryFormatterTag.ObjectBegin)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at BeginObject. Expected {BinaryFormatterTag.ObjectBegin}, found {tag}.");
            }

            var depth = (int)ReadVarint32();
            if (depth != _nodeDepth)
            {
                throw new InvalidOperationException(
                    $"Depth mismatch at BeginObject. Expected {_nodeDepth}, found {depth}.");
            }

            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndObject()
        {
            _nodeDepth--;

            var depth = (int)ReadVarint32();
            if (depth != _nodeDepth)
            {
                throw new InvalidOperationException(
                    $"Depth mismatch at EndObject. Expected {_nodeDepth}, found {depth}.");
            }

            var tag = (BinaryFormatterTag)_reader.ReadByte();
            if (tag != BinaryFormatterTag.ObjectEnd)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at EndObject. Expected {BinaryFormatterTag.ObjectEnd}, found {tag}.");
            }
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            var tag = (BinaryFormatterTag)_reader.ReadByte();
            if (tag != BinaryFormatterTag.ArrayBegin)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at BeginArray. Expected {BinaryFormatterTag.ArrayBegin}, found {tag}.");
            }

            length = (int)ReadVarint32();

            var depth = (int)ReadVarint32();
            if (depth != _nodeDepth)
            {
                throw new InvalidOperationException(
                    $"Depth mismatch at BeginArray. Expected {_nodeDepth}, found {depth}.");
            }

            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndArray()
        {
            _nodeDepth--;

            var depth = (int)ReadVarint32();
            if (depth != _nodeDepth)
            {
                throw new InvalidOperationException(
                    $"Depth mismatch at EndArray. Expected {_nodeDepth}, found {depth}.");
            }

            var tag = (BinaryFormatterTag)_reader.ReadByte();
            if (tag != BinaryFormatterTag.ArrayEnd)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at EndArray. Expected {BinaryFormatterTag.ArrayEnd}, found {tag}.");
            }
        }

        /// <inheritdoc />
        public override void Format(ref int value)
        {
            // Decode zigzag encoding to recover signed integer
            uint encoded = ReadVarint32();
            value = (int)((encoded >> 1) ^ -(int)(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref sbyte value)
        {
            // Decode zigzag encoding to recover signed byte
            int encoded = _reader.ReadByte();
            value = (sbyte)((encoded >> 1) ^ -(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref short value)
        {
            // Decode zigzag encoding to recover signed short
            uint encoded = ReadVarint32();
            value = (short)((encoded >> 1) ^ -(int)(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref long value)
        {
            // Decode zigzag encoding to recover signed long
            ulong encoded = ReadVarint64();
            long decoded = (long)(encoded >> 1);
            if ((encoded & 1) == 1)
                decoded = ~decoded;
            value = decoded;
        }

        /// <inheritdoc />
        public override void Format(ref byte value)
        {
            value = _reader.ReadByte();
        }

        /// <inheritdoc />
        public override void Format(ref ushort value)
        {
            value = (ushort)ReadVarint32();
        }

        /// <inheritdoc />
        public override void Format(ref uint value)
        {
            value = ReadVarint32();
        }

        /// <inheritdoc />
        public override void Format(ref ulong value)
        {
            value = ReadVarint64();
        }

        /// <inheritdoc />
        public override void Format(ref bool value)
        {
            var byteValue = _reader.ReadByte();
            value = byteValue != 0;
        }

        /// <inheritdoc />
        public override void Format(ref float value)
        {
            value = _reader.ReadSingle();
        }

        /// <inheritdoc />
        public override void Format(ref double value)
        {
            value = _reader.ReadDouble();
        }

        /// <inheritdoc />
        public override void Format(ref string str)
        {
            var length = ReadVarint32();
            if (length == 0)
            {
                str = string.Empty;
                return;
            }

            var bytes = _reader.ReadBytes((int)length);
            str = Encoding.UTF8.GetString(bytes);
        }

        /// <inheritdoc />
        public override void Format(ref byte[] data)
        {
            var length = ReadVarint32();
            if (length == 0)
            {
                data = null;
                return;
            }

            data = _reader.ReadBytes((int)length);
        }

        /// <inheritdoc />
        public override void Format(ref UnityEngine.Object unityObject)
        {
            var index = ReadVarint32();
            unityObject = ResolveReference((int)index);
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

        /// <summary>Reads a 64-bit unsigned integer using variable-length decoding.</summary>
        /// <returns>The decoded value.</returns>
        private ulong ReadVarint64()
        {
            ulong value = 0;
            int shift = 0;
            byte b;
            do
            {
                b = _reader.ReadByte();
                value |= (ulong)(b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return value;
        }
    }
}
