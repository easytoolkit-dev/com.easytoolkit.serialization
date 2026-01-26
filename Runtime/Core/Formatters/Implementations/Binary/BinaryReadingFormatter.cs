using System;
using System.Collections.Generic;
using EasyToolKit.Core.Pooling;
using EasyToolKit.Serialization.Utilities;

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
        private readonly Dictionary<int, Type> _typeById;
        private BinaryFormatterOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryReadingFormatter"/> class
        /// for object pool reuse. Use <see cref="SetBuffer"/> to set the data.
        /// </summary>
        public BinaryReadingFormatter()
        {
            _buffer = Array.Empty<byte>();
            _nodeDepth = 0;
            _position = 0;
            _typeById = new Dictionary<int, Type>();
        }

        /// <inheritdoc />
        public override SerializationFormat FormatType => SerializationFormat.Binary;

        /// <inheritdoc />
        public override void SetBuffer(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer.ToArray();
            _position = 0;
            _nodeDepth = 0;
            _typeById.Clear();
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
            if (GetRemainingLength() == 0)
            {
                if (Settings.ReturnDefaultOnStreamEnd)
                    return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.MemberBegin, "begin member");
            if ((_options & BinaryFormatterOptions.IncludeMemberNames) != 0)
            {
                var length = ReadUInt32Optimized();
                if (length > 0)
                {
                    var readName = ReadString((int)length);
                    // Verify name matches if provided (skip verification for auto-generated names starting with '$')
                    if (!string.IsNullOrEmpty(name) && readName != name)
                    {
                        throw new DataFormatException(
                            $"Member name mismatch. Expected '{name}', found '{readName}'.");
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void BeginObject(Type type)
        {
            if (GetRemainingLength() == 0)
            {
                if (Settings.ReturnDefaultOnStreamEnd)
                    return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.ObjectBegin, "begin object");

            if ((_options & BinaryFormatterOptions.IncludeObjectType) != 0)
            {
                var typeTag = ReadTag();
                switch (typeTag)
                {
                    case BinaryFormatterTag.NullType:
                        if (type != null)
                        {
                            throw new DataFormatException(
                                $"Type mismatch in binary data. Expected type '{type}', found null.");
                        }

                        break;
                    case BinaryFormatterTag.TypeId:
                    {
                        var typeId = (int)ReadUInt32Optimized();
                        if (!_typeById.TryGetValue(typeId, out var foundType))
                        {
                            throw new DataFormatException(
                                $"Type ID {typeId} not found in type dictionary.");
                        }

                        if (foundType != type)
                        {
                            throw new DataFormatException(
                                $"Type mismatch in binary data. Expected type '{type}', found '{foundType}'.");
                        }

                        break;
                    }
                    case BinaryFormatterTag.TypeName:
                    {
                        var typeNameLength = ReadUInt32Optimized();
                        var typeName = ReadString((int)typeNameLength);
                        var foundType = SerializedTypeUtility.NameToType(typeName);
                        if (foundType != type)
                        {
                            throw new DataFormatException(
                                $"Type mismatch in binary data. Expected type '{type}', found '{foundType}'.");
                        }

                        _typeById[_typeById.Count] = type;
                        break;
                    }
                    default:
                        throw new DataFormatException(
                            $"Invalid type tag: {typeTag}. Expected NullType, TypeId, or TypeName.");
                }
            }

            ReadAndValidateNodeDepth(nameof(BeginObject));
            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndObject()
        {
            if (GetRemainingLength() == 0)
            {
                if (Settings.ReturnDefaultOnStreamEnd)
                    return;
            }

            _nodeDepth--;
            ReadAndValidateNodeDepth(nameof(EndObject));
            ReadAndValidateOptionTag(BinaryFormatterTag.ObjectEnd, "end object");
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            if (GetRemainingLength() == 0)
            {
                if (Settings.ReturnDefaultOnStreamEnd)
                    return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.ArrayBegin, "begin array");

            length = (int)ReadUInt32Optimized();
            ReadAndValidateNodeDepth(nameof(BeginArray));

            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndArray()
        {
            if (GetRemainingLength() == 0)
            {
                if (Settings.ReturnDefaultOnStreamEnd)
                    return;
            }

            _nodeDepth--;
            ReadAndValidateNodeDepth(nameof(EndArray));
            ReadAndValidateOptionTag(BinaryFormatterTag.ArrayEnd, "end array");
        }

        /// <inheritdoc />
        protected override void Format(ref int value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Int32, "int");
            // Decode zigzag encoding to recover signed integer
            uint encoded;
            if ((_options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
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
        protected override void Format(ref sbyte value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Int8, "sbyte");
            // Decode zigzag encoding to recover signed byte
            int encoded = ReadByte();
            value = (sbyte)((encoded >> 1) ^ -(encoded & 1));
        }

        /// <inheritdoc />
        protected override void Format(ref short value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Int16, "short");
            // Decode zigzag encoding to recover signed short
            uint encoded;
            if ((_options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
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
        protected override void Format(ref long value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Int64, "long");
            // Decode zigzag encoding to recover signed long
            ulong encoded;
            if ((_options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
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
        protected override void Format(ref byte value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UInt8, "byte");
            value = ReadByte();
        }

        /// <inheritdoc />
        protected override void Format(ref ushort value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UInt16, "ushort");

            if ((_options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                value = (ushort)ReadVarint32();
            }
            else
            {
                value = ReadUInt16Fixed();
            }
        }

        /// <inheritdoc />
        protected override void Format(ref uint value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UInt32, "uint");

            if ((_options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                value = ReadVarint32();
            }
            else
            {
                value = ReadUInt32Fixed();
            }
        }

        /// <inheritdoc />
        protected override void Format(ref ulong value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UInt64, "ulong");

            if ((_options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                value = ReadVarint64();
            }
            else
            {
                value = ReadUInt64Fixed();
            }
        }

        /// <inheritdoc />
        protected override void Format(ref bool value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Boolean, "bool");
            var byteValue = ReadByte();
            value = byteValue != 0;
        }

        /// <inheritdoc />
        protected override void Format(ref float value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Single, "float");
            value = ReadSingle();
        }

        /// <inheritdoc />
        protected override void Format(ref double value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Double, "double");
            value = ReadDouble();
        }

        /// <inheritdoc />
        protected override void Format(ref string str)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.String, "string");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                str = string.Empty;
                return;
            }

            if ((_options & BinaryFormatterOptions.EnableDirectMemoryCopy) != 0)
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
        protected override void Format(ref byte[] data)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.ByteArray, "byte array");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                data = Array.Empty<byte>();
                return;
            }

            data = ReadBytes((int)length).ToArray();
        }

        /// <inheritdoc />
        protected override void Format(ref sbyte[] data)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.SByteArray, "sbyte array");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                data = Array.Empty<sbyte>();
                return;
            }

            data = ReadPrimitiveArray<sbyte>((int)length);
        }

        /// <inheritdoc />
        protected override void Format(ref short[] data)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Int16Array, "short array");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                data = Array.Empty<short>();
                return;
            }

            data = ReadPrimitiveArray<short>((int)length);
        }

        /// <inheritdoc />
        protected override void Format(ref int[] data)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Int32Array, "int array");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                data = Array.Empty<int>();
                return;
            }

            data = ReadPrimitiveArray<int>((int)length);
        }

        /// <inheritdoc />
        protected override void Format(ref long[] data)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.Int64Array, "long array");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                data = Array.Empty<long>();
                return;
            }

            data = ReadPrimitiveArray<long>((int)length);
        }

        /// <inheritdoc />
        protected override void Format(ref ushort[] data)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UInt16Array, "ushort array");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                data = Array.Empty<ushort>();
                return;
            }

            data = ReadPrimitiveArray<ushort>((int)length);
        }

        /// <inheritdoc />
        protected override void Format(ref uint[] data)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UInt32Array, "uint array");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                data = Array.Empty<uint>();
                return;
            }

            data = ReadPrimitiveArray<uint>((int)length);
        }

        /// <inheritdoc />
        protected override void Format(ref ulong[] data)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UInt64Array, "ulong array");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                data = Array.Empty<ulong>();
                return;
            }

            data = ReadPrimitiveArray<ulong>((int)length);
        }

        /// <inheritdoc />
        protected override void Format(ref UnityEngine.Object unityObject)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UnityObjectRef, "Unity object reference");
            var index = ReadUInt32Optimized();
            unityObject = ResolveReference((int)index);
        }

        /// <inheritdoc />
        protected override void FormatGenericPrimitive<T>(ref T value)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UnmanagedValue, "unmanaged value");
            value = ReadPrimitiveValue<T>();
        }

        /// <inheritdoc />
        protected override void FormatGenericPrimitive<T>(ref T[] data)
        {
            ReadAndValidateOptionTag(BinaryFormatterTag.UnmanagedArray, "unmanaged array");
            var length = ReadUInt32Optimized();
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
            _typeById.Clear();
            PoolUtility.ReleaseObject(this);
            base.Dispose();
        }

        protected override void OnSettingsChanged(DataFormatterSettings settings)
        {
            if (settings is not BinaryFormatterSettings binaryFormatterSettings)
            {
                throw new ArgumentException(
                    $"Settings must be of type {nameof(BinaryFormatterSettings)}. " +
                    $"Received type: {settings}. " +
                    $"Please provide a valid BinaryFormatterSettings instance.",
                    nameof(settings));
            }

            _options = binaryFormatterSettings.Options;
        }
    }
}
