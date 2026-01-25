using System;
using EasyToolKit.Core.Pooling;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Binary reading formatter implementation. Deserializes data from a binary format
    /// using length-prefixed field names and varint encoding.
    /// </summary>
    public sealed partial class BinaryReadingFormatter : ReadingFormatterBase
    {
        private int _position;
        private byte[] _buffer;
        private int _nodeDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryReadingFormatter"/> class
        /// for object pool reuse. Use <see cref="SetBuffer"/> to set the data.
        /// </summary>
        public BinaryReadingFormatter()
        {
            _buffer = Array.Empty<byte>();
            _nodeDepth = 0;
            _position = 0;
        }

        /// <inheritdoc />
        public override SerializationFormat Type => SerializationFormat.Binary;

        /// <inheritdoc />
        public override void SetBuffer(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer.ToArray();
            _position = 0;
            _nodeDepth = 0;
        }

        /// <inheritdoc />
        public override ReadOnlySpan<byte> GetBuffer() => _buffer;

        /// <inheritdoc />
        public override int GetPosition() => _position;

        /// <inheritdoc />
        public override int GetRemainingLength() => _buffer.Length - _position;

        /// <inheritdoc />
        protected override void BeginMember(string name, bool isInArrayContext)
        {
            // Skip reading member data in Array context
            if (isInArrayContext)
            {
                return;
            }

            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.MemberBegin)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at BeginMember. Expected {BinaryFormatterTag.MemberBegin}, found {tag}.");
            }

            var length = ReadVarint32();
            if (length > 0)
            {
                var readName = ReadString((int)length);
                // Verify name matches if provided (skip verification for auto-generated names starting with '$')
                if (!string.IsNullOrEmpty(name) && !name.StartsWith("$") && readName != name)
                {
                    throw new InvalidOperationException(
                        $"Member name mismatch. Expected '{name}', found '{readName}'.");
                }
            }
        }

        /// <inheritdoc />
        protected override void BeginObject()
        {
            var tag = (BinaryFormatterTag)ReadByte();
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

            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.ObjectEnd)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at EndObject. Expected {BinaryFormatterTag.ObjectEnd}, found {tag}.");
            }
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            var tag = (BinaryFormatterTag)ReadByte();
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

            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.ArrayEnd)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at EndArray. Expected {BinaryFormatterTag.ArrayEnd}, found {tag}.");
            }
        }

        /// <inheritdoc />
        public override void Format(ref int value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.Int32)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for int. Expected {BinaryFormatterTag.Int32}, found {tag}.");
            }
            // Decode zigzag encoding to recover signed integer
            uint encoded = ReadVarint32();
            value = (int)((encoded >> 1) ^ -(int)(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref sbyte value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.Int8)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for sbyte. Expected {BinaryFormatterTag.Int8}, found {tag}.");
            }
            // Decode zigzag encoding to recover signed byte
            int encoded = ReadByte();
            value = (sbyte)((encoded >> 1) ^ -(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref short value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.Int16)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for short. Expected {BinaryFormatterTag.Int16}, found {tag}.");
            }
            // Decode zigzag encoding to recover signed short
            uint encoded = ReadVarint32();
            value = (short)((encoded >> 1) ^ -(int)(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref long value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.Int64)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for long. Expected {BinaryFormatterTag.Int64}, found {tag}.");
            }
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
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.UInt8)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for byte. Expected {BinaryFormatterTag.UInt8}, found {tag}.");
            }
            value = ReadByte();
        }

        /// <inheritdoc />
        public override void Format(ref ushort value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.UInt16)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for ushort. Expected {BinaryFormatterTag.UInt16}, found {tag}.");
            }
            value = (ushort)ReadVarint32();
        }

        /// <inheritdoc />
        public override void Format(ref uint value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.UInt32)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for uint. Expected {BinaryFormatterTag.UInt32}, found {tag}.");
            }
            value = ReadVarint32();
        }

        /// <inheritdoc />
        public override void Format(ref ulong value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.UInt64)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for ulong. Expected {BinaryFormatterTag.UInt64}, found {tag}.");
            }
            value = ReadVarint64();
        }

        /// <inheritdoc />
        public override void Format(ref bool value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.Boolean)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for bool. Expected {BinaryFormatterTag.Boolean}, found {tag}.");
            }
            var byteValue = ReadByte();
            value = byteValue != 0;
        }

        /// <inheritdoc />
        public override void Format(ref float value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.Single)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for float. Expected {BinaryFormatterTag.Single}, found {tag}.");
            }
            value = ReadSingle();
        }

        /// <inheritdoc />
        public override void Format(ref double value)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.Double)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for double. Expected {BinaryFormatterTag.Double}, found {tag}.");
            }
            value = ReadDouble();
        }

        /// <inheritdoc />
        public override void Format(ref string str)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.String)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for string. Expected {BinaryFormatterTag.String}, found {tag}.");
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                str = string.Empty;
                return;
            }

            str = ReadString((int)length);
        }

        /// <inheritdoc />
        public override void Format(ref byte[] data)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.ByteArray)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for byte array. Expected {BinaryFormatterTag.ByteArray}, found {tag}.");
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                data = Array.Empty<byte>();
                return;
            }

            data = ReadBytes((int)length).ToArray();
        }

        /// <inheritdoc />
        public override void Format(ref UnityEngine.Object unityObject)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.UnityObjectRef)
            {
                throw new InvalidOperationException(
                    $"Invalid type tag for Unity object reference. Expected {BinaryFormatterTag.UnityObjectRef}, found {tag}.");
            }
            var index = ReadVarint32();
            unityObject = ResolveReference((int)index);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _position = 0;
            _nodeDepth = 0;
            _buffer = Array.Empty<byte>();
            PoolUtility.ReleaseObject(this);
            base.Dispose();
        }
    }
}
