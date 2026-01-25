using System;
using EasyToolKit.Core.Pooling;

namespace EasyToolKit.Serialization.Formatters.Implementations
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

        /// <summary>
        /// Gets the formatter options from settings, or Default if settings is not a BinaryFormatterSettings.
        /// </summary>
        private BinaryFormatterOptions Options =>
            ((BinaryFormatterSettings)Settings)?.Options ?? BinaryFormatterOptions.Default;

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

            if ((Options & BinaryFormatterOptions.IncludeMemberNames) != 0)
            {
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
            // When disabled, members are identified by position only
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
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Int32)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for int. Expected {BinaryFormatterTag.Int32}, found {tag}.");
                }
            }
            // Decode zigzag encoding to recover signed integer
            uint encoded;
            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                encoded = ReadVarint32();
            }
            else
            {
                encoded = ReadUInt32Fixed();
            }
            value = (int)((encoded >> 1) ^ -(int)(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref sbyte value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Int8)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for sbyte. Expected {BinaryFormatterTag.Int8}, found {tag}.");
                }
            }
            // Decode zigzag encoding to recover signed byte
            int encoded = ReadByte();
            value = (sbyte)((encoded >> 1) ^ -(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref short value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Int16)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for short. Expected {BinaryFormatterTag.Int16}, found {tag}.");
                }
            }
            // Decode zigzag encoding to recover signed short
            uint encoded;
            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                encoded = ReadVarint32();
            }
            else
            {
                encoded = ReadUInt16Fixed();
            }
            value = (short)((encoded >> 1) ^ -(int)(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref long value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Int64)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for long. Expected {BinaryFormatterTag.Int64}, found {tag}.");
                }
            }
            // Decode zigzag encoding to recover signed long
            ulong encoded;
            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                encoded = ReadVarint64();
            }
            else
            {
                encoded = ReadUInt64Fixed();
            }
            long decoded = (long)(encoded >> 1);
            if ((encoded & 1) == 1)
                decoded = ~decoded;
            value = decoded;
        }

        /// <inheritdoc />
        public override void Format(ref byte value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.UInt8)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for byte. Expected {BinaryFormatterTag.UInt8}, found {tag}.");
                }
            }
            value = ReadByte();
        }

        /// <inheritdoc />
        public override void Format(ref ushort value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.UInt16)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for ushort. Expected {BinaryFormatterTag.UInt16}, found {tag}.");
                }
            }

            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                value = (ushort)ReadVarint32();
            }
            else
            {
                value = ReadUInt16Fixed();
            }
        }

        /// <inheritdoc />
        public override void Format(ref uint value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.UInt32)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for uint. Expected {BinaryFormatterTag.UInt32}, found {tag}.");
                }
            }

            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                value = ReadVarint32();
            }
            else
            {
                value = ReadUInt32Fixed();
            }
        }

        /// <inheritdoc />
        public override void Format(ref ulong value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.UInt64)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for ulong. Expected {BinaryFormatterTag.UInt64}, found {tag}.");
                }
            }

            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                value = ReadVarint64();
            }
            else
            {
                value = ReadUInt64Fixed();
            }
        }

        /// <inheritdoc />
        public override void Format(ref bool value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Boolean)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for bool. Expected {BinaryFormatterTag.Boolean}, found {tag}.");
                }
            }
            var byteValue = ReadByte();
            value = byteValue != 0;
        }

        /// <inheritdoc />
        public override void Format(ref float value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Single)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for float. Expected {BinaryFormatterTag.Single}, found {tag}.");
                }
            }
            value = ReadSingle();
        }

        /// <inheritdoc />
        public override void Format(ref double value)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Double)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for double. Expected {BinaryFormatterTag.Double}, found {tag}.");
                }
            }
            value = ReadDouble();
        }

        /// <inheritdoc />
        public override void Format(ref string str)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.String)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for string. Expected {BinaryFormatterTag.String}, found {tag}.");
                }
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                str = string.Empty;
                return;
            }

            if ((Options & BinaryFormatterOptions.EnableDirectMemoryCopy) != 0)
            {
                // Length is char count (current implementation)
                str = ReadString((int)length);
            }
            else
            {
                // Length is byte count (UTF-8)
                var utf8Bytes = ReadBytes((int)length);
                str = System.Text.Encoding.UTF8.GetString(utf8Bytes);
            }
        }

        /// <inheritdoc />
        public override void Format(ref byte[] data)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.ByteArray)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for byte array. Expected {BinaryFormatterTag.ByteArray}, found {tag}.");
                }
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
        public override void Format(ref sbyte[] data)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.SByteArray)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for sbyte array. Expected {BinaryFormatterTag.SByteArray}, found {tag}.");
                }
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                data = Array.Empty<sbyte>();
                return;
            }

            data = ReadPrimitiveArray<sbyte>((int)length);
        }

        /// <inheritdoc />
        public override void Format(ref short[] data)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Int16Array)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for short array. Expected {BinaryFormatterTag.Int16Array}, found {tag}.");
                }
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                data = Array.Empty<short>();
                return;
            }

            data = ReadPrimitiveArray<short>((int)length);
        }

        /// <inheritdoc />
        public override void Format(ref int[] data)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Int32Array)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for int array. Expected {BinaryFormatterTag.Int32Array}, found {tag}.");
                }
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                data = Array.Empty<int>();
                return;
            }

            data = ReadPrimitiveArray<int>((int)length);
        }

        /// <inheritdoc />
        public override void Format(ref long[] data)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.Int64Array)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for long array. Expected {BinaryFormatterTag.Int64Array}, found {tag}.");
                }
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                data = Array.Empty<long>();
                return;
            }

            data = ReadPrimitiveArray<long>((int)length);
        }

        /// <inheritdoc />
        public override void Format(ref ushort[] data)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.UInt16Array)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for ushort array. Expected {BinaryFormatterTag.UInt16Array}, found {tag}.");
                }
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                data = Array.Empty<ushort>();
                return;
            }

            data = ReadPrimitiveArray<ushort>((int)length);
        }

        /// <inheritdoc />
        public override void Format(ref uint[] data)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.UInt32Array)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for uint array. Expected {BinaryFormatterTag.UInt32Array}, found {tag}.");
                }
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                data = Array.Empty<uint>();
                return;
            }

            data = ReadPrimitiveArray<uint>((int)length);
        }

        /// <inheritdoc />
        public override void Format(ref ulong[] data)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.UInt64Array)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for ulong array. Expected {BinaryFormatterTag.UInt64Array}, found {tag}.");
                }
            }
            var length = ReadVarint32();
            if (length == 0)
            {
                data = Array.Empty<ulong>();
                return;
            }

            data = ReadPrimitiveArray<ulong>((int)length);
        }

        /// <inheritdoc />
        public override void Format(ref UnityEngine.Object unityObject)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != BinaryFormatterTag.UnityObjectRef)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for Unity object reference. Expected {BinaryFormatterTag.UnityObjectRef}, found {tag}.");
                }
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
