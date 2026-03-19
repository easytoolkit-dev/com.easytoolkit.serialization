using System;
using System.Collections.Generic;
using System.IO;
using EasyToolkit.Core.Pooling;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Utilities;

namespace EasyToolkit.Serialization.Formatters.Implementations
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
        private bool _returnDefaultOnEmptyMember;
        private readonly Stack<bool> _missingMemberStack;
        private bool _isInMissingMember;
        private bool _isNextMemberMissing;

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
            _missingMemberStack = new Stack<bool>();
            _isInMissingMember = false;
            _isNextMemberMissing = false;
        }

        /// <inheritdoc />
        public override SerializationFormat FormatType => SerializationFormat.Binary;

        /// <inheritdoc />
        protected override void SetBuffer(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer.ToArray();
            _position = 0;
            _nodeDepth = 0;
            _typeById.Clear();
            _missingMemberStack.Clear();
            _isInMissingMember = false;
            _isNextMemberMissing = false;
        }

        /// <inheritdoc />
        protected override ReadOnlySpan<byte> GetBuffer() => _buffer;

        /// <inheritdoc />
        protected override int GetPosition() => _position;

        /// <inheritdoc />
        protected override int GetRemainingLength() => _buffer.Length - _position;

        /// <inheritdoc />
        protected override void BeginMember(string name)
        {
            // Check if we should skip reading this member
            if (ShouldSkipMemberReading())
            {
                // Mark this member as missing and skip reading
                MarkNextMemberMissing();
                return;
            }

            // If we're at the end of stream, throw an exception (data corruption)
            if (IsEndOfStream())
            {
                throw new EndOfStreamException(
                    "Attempted to read member but reached end of stream. " +
                    "This may indicate corrupted serialization data.");
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
        protected override void BeginObject(Type expectedType)
        {
            // Check if we're in a missing member scope
            if (_returnDefaultOnEmptyMember && IsInMissingMemberScope())
            {
                // Push missing member state for nested scope
                EnterMissingMemberScope();
                return;
            }

            // Check if we've reached the end of stream or ObjectEnd (missing member scenario)
            if (_returnDefaultOnEmptyMember && (IsEndOfStream() || PeekTag() == BinaryFormatterTag.ObjectEnd))
            {
                // Enter missing member scope
                EnterMissingMemberScope();
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.ObjectBegin, "begin object");
            ReadAndValidateType(expectedType);
            ReadAndValidateNodeDepth(nameof(BeginObject));
            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndObject()
        {
            // Check if we're in a missing member scope
            if (_returnDefaultOnEmptyMember && IsInMissingMemberScope())
            {
                // Exit missing member scope
                ExitMissingMemberScope();
                return;
            }

            _nodeDepth--;
            ReadAndValidateOptionTag(BinaryFormatterTag.ObjectEnd, "end object");
            ReadAndValidateNodeDepth(nameof(EndObject));
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            // Check if we're in a missing member scope
            if (_returnDefaultOnEmptyMember && IsInMissingMemberScope())
            {
                // Push missing member state for nested scope
                EnterMissingMemberScope();
                length = 0;
                return;
            }

            // Check if we've reached the end of stream or ObjectEnd (missing member scenario)
            if (_returnDefaultOnEmptyMember && (IsEndOfStream() || PeekTag() == BinaryFormatterTag.ObjectEnd))
            {
                // Enter missing member scope
                EnterMissingMemberScope();
                length = 0;
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
            // Check if we're in a missing member scope
            if (_returnDefaultOnEmptyMember && IsInMissingMemberScope())
            {
                // Exit missing member scope
                ExitMissingMemberScope();
                return;
            }

            _nodeDepth--;
            ReadAndValidateOptionTag(BinaryFormatterTag.ArrayEnd, "end array");
            ReadAndValidateNodeDepth(nameof(EndArray));
        }

        /// <inheritdoc />
        protected override void Format(ref int value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.Int8, "sbyte");
            // Decode zigzag encoding to recover signed byte
            int encoded = ReadByte();
            value = (sbyte)((encoded >> 1) ^ -(encoded & 1));
        }

        /// <inheritdoc />
        protected override void Format(ref short value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.UInt8, "byte");
            value = ReadByte();
        }

        /// <inheritdoc />
        protected override void Format(ref ushort value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.Boolean, "bool");
            var byteValue = ReadByte();
            value = byteValue != 0;
        }

        /// <inheritdoc />
        protected override void Format(ref bool[] data)
        {
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<bool>();
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.BooleanArray, "bool array");
            var length = ReadUInt32Optimized();
            if (length == 0)
            {
                data = Array.Empty<bool>();
                return;
            }

            data = new bool[length];

            // Unpack bytes into bool array (8 bools per byte)
            var byteCount = (length + 7) / 8;
            if (_position + byteCount > _buffer.Length)
            {
                throw new EndOfStreamException(
                    $"Attempted to read {byteCount} bytes but only {_buffer.Length - _position} bytes available.");
            }

            int boolIndex = 0;
            for (int byteIndex = 0; byteIndex < byteCount && boolIndex < length; byteIndex++)
            {
                byte currentByte = _buffer[_position++];
                for (int bitIndex = 0; bitIndex < 8 && boolIndex < length; bitIndex++)
                {
                    data[boolIndex++] = (currentByte & (1 << bitIndex)) != 0;
                }
            }
        }

        /// <inheritdoc />
        protected override void Format(ref float value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.Single, "float");
            value = ReadSingle();
        }

        /// <inheritdoc />
        protected override void Format(ref double value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.Double, "double");
            value = ReadDouble();
        }

        /// <inheritdoc />
        protected override void Format(ref decimal value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.Decimal, "decimal");
            value = ReadDecimal();
        }

        /// <inheritdoc />
        protected override void Format(ref string str)
        {
            if (ShouldAssignDefaultValue())
            {
                str = default;
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<byte>();
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<sbyte>();
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<short>();
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<int>();
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<long>();
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<ushort>();
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<uint>();
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<ulong>();
                return;
            }

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
            if (ShouldAssignDefaultValue())
            {
                unityObject = default;
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.UnityObjectRef, "Unity object reference");
            var index = ReadUInt32Optimized();
            unityObject = ResolveReference((int)index);
        }

        /// <inheritdoc />
        protected override void FormatNullable(ref bool isNull)
        {
            if (ShouldAssignDefaultValue())
            {
                isNull = true; // Treat as null
                return;
            }

            var byteValue = ReadByte();
            isNull = byteValue != 0;
        }

        /// <inheritdoc />
        protected override void FormatGenericPrimitive<T>(ref T value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            ReadAndValidateOptionTag(BinaryFormatterTag.UnmanagedValue, "unmanaged value");
            value = ReadPrimitiveValue<T>();
        }

        /// <inheritdoc />
        protected override void FormatGenericPrimitive<T>(ref T[] data)
        {
            if (ShouldAssignDefaultValue())
            {
                data = Array.Empty<T>();
                return;
            }

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
        protected override Type PeekType(Type expectedType)
        {
            var originalPosition = _position;
            ReadAndValidateOptionTag(BinaryFormatterTag.ObjectBegin, "begin object");
            var enableValidate = expectedType != null;
            var type = ReadAndValidateType(expectedType, enableValidate);
            _position = originalPosition;
            return type;
        }

        /// <inheritdoc />
        protected override void Dispose()
        {
            _position = 0;
            _nodeDepth = 0;
            _buffer = Array.Empty<byte>();
            _typeById.Clear();
            _missingMemberStack.Clear();
            _isInMissingMember = false;
            _isNextMemberMissing = false;
            PoolUtility.ReleaseObject(this);
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
            _returnDefaultOnEmptyMember = binaryFormatterSettings.ReturnDefaultOnEmptyMember;
        }
    }
}
