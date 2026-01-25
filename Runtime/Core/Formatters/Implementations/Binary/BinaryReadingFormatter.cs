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
        public override SerializationFormat FormatType => SerializationFormat.Binary;

        /// <summary>
        /// Gets the formatter options from settings, or Default if settings is not a BinaryFormatterSettings.
        /// </summary>
        private BinaryFormatterOptions Options =>
            ((BinaryFormatterSettings)Settings)?.Options ?? BinaryFormatterOptions.Default;

        /// <summary>
        /// Reads and validates a tag when IncludeTypeTags option is enabled.
        /// </summary>
        /// <param name="expectedTag">The expected tag value.</param>
        /// <param name="context">Context description for error messages.</param>
        private void ReadAndValidateTypeTag(BinaryFormatterTag expectedTag, string context)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = (BinaryFormatterTag)ReadByte();
                if (tag != expectedTag)
                {
                    throw new InvalidOperationException(
                        $"Invalid type tag for {context}. Expected {expectedTag}, found {tag}.");
                }
            }
        }

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
        protected override void BeginMember(string name)
        {
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
            ReadAndValidateTypeTag(BinaryFormatterTag.Int32, "int");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.Int8, "sbyte");
            // Decode zigzag encoding to recover signed byte
            int encoded = ReadByte();
            value = (sbyte)((encoded >> 1) ^ -(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref short value)
        {
            ReadAndValidateTypeTag(BinaryFormatterTag.Int16, "short");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.Int64, "long");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.UInt8, "byte");
            value = ReadByte();
        }

        /// <inheritdoc />
        public override void Format(ref ushort value)
        {
            ReadAndValidateTypeTag(BinaryFormatterTag.UInt16, "ushort");

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
            ReadAndValidateTypeTag(BinaryFormatterTag.UInt32, "uint");

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
            ReadAndValidateTypeTag(BinaryFormatterTag.UInt64, "ulong");

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
            ReadAndValidateTypeTag(BinaryFormatterTag.Boolean, "bool");
            var byteValue = ReadByte();
            value = byteValue != 0;
        }

        /// <inheritdoc />
        public override void Format(ref float value)
        {
            ReadAndValidateTypeTag(BinaryFormatterTag.Single, "float");
            value = ReadSingle();
        }

        /// <inheritdoc />
        public override void Format(ref double value)
        {
            ReadAndValidateTypeTag(BinaryFormatterTag.Double, "double");
            value = ReadDouble();
        }

        /// <inheritdoc />
        public override void Format(ref string str)
        {
            ReadAndValidateTypeTag(BinaryFormatterTag.String, "string");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.ByteArray, "byte array");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.SByteArray, "sbyte array");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.Int16Array, "short array");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.Int32Array, "int array");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.Int64Array, "long array");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.UInt16Array, "ushort array");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.UInt32Array, "uint array");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.UInt64Array, "ulong array");
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
            ReadAndValidateTypeTag(BinaryFormatterTag.UnityObjectRef, "Unity object reference");
            var index = ReadVarint32();
            unityObject = ResolveReference((int)index);
        }

        /// <inheritdoc />
        public override void FormatGenericPrimitive<T>(ref T value)
        {
            ReadAndValidateTypeTag(BinaryFormatterTag.UnmanagedValue, "unmanaged value");
            value = ReadPrimitiveValue<T>();
        }

        /// <inheritdoc />
        public override void FormatGenericPrimitive<T>(ref T[] data)
        {
            ReadAndValidateTypeTag(BinaryFormatterTag.UnmanagedArray, "unmanaged array");
            var length = ReadVarint32();
            if (length == 0)
            {
                data = Array.Empty<T>();
                return;
            }

            data = ReadPrimitiveArray<T>((int)length);
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
