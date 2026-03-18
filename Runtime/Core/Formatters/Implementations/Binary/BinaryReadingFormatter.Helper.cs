using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Utilities;

namespace EasyToolkit.Serialization.Formatters.Implementations
{
    public sealed partial class BinaryReadingFormatter
    {
        /// <summary>
        /// Reads a string from raw char memory (2 bytes per char) with the specified char count.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe string ReadString(int charCount)
        {
            if (charCount < 0)
                throw new ArgumentOutOfRangeException(nameof(charCount), "Char count cannot be negative.");

            int byteCount = charCount * sizeof(char);
            if (_position + byteCount > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {byteCount} bytes but only {_buffer.Length - _position} bytes available.");

            fixed (byte* bytePtr = &_buffer[_position])
            {
                _position += byteCount;
                return new string((char*)bytePtr, 0, charCount);
            }
        }

        /// <summary>
        /// Reads a single byte from the buffer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte ReadByte()
        {
            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");
            return _buffer[_position++];
        }

        /// <summary>
        /// Reads the specified number of bytes from the buffer.
        /// </summary>
        private ReadOnlySpan<byte> ReadBytes(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            if (_position + count > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {count} bytes but only {_buffer.Length - _position} bytes available.");

            var result = _buffer.AsSpan(_position, count);
            _position += count;
            return result;
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer using variable-length decoding.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadVarint32()
        {
            uint value = 0;

            // Unrolled loop for performance (handles up to 5 bytes inline)
            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            byte b = _buffer[_position++];
            value = (uint)(b & 0x7F);
            if ((b & 0x80) == 0) return value;

            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            b = _buffer[_position++];
            value |= (uint)(b & 0x7F) << 7;
            if ((b & 0x80) == 0) return value;

            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            b = _buffer[_position++];
            value |= (uint)(b & 0x7F) << 14;
            if ((b & 0x80) == 0) return value;

            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            b = _buffer[_position++];
            value |= (uint)(b & 0x7F) << 21;
            if ((b & 0x80) == 0) return value;

            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            b = _buffer[_position++];
            value |= (uint)(b & 0x7F) << 28;
            if ((b & 0x80) == 0) return value;

            throw new InvalidDataException("Invalid varint32: too many bytes.");
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer using variable-length decoding.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong ReadVarint64()
        {
            ulong value = 0;
            int shift = 0;
            byte b;

            // Optimized loop for 64-bit varint
            do
            {
                if (_position >= _buffer.Length)
                    throw new EndOfStreamException("Attempted to read past the end of the buffer.");

                b = _buffer[_position++];

                if (shift >= 64)
                    throw new InvalidDataException("Invalid varint64: too many bytes.");

                value |= (ulong)(b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);

            return value;
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer using fixed-width encoding (2 bytes).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort ReadUInt16Fixed()
        {
            const int size = 2;
            if (_position + size > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {size} bytes but only {_buffer.Length - _position} bytes available.");

            ushort value = (ushort)(
                _buffer[_position] |
                (_buffer[_position + 1] << 8));
            _position += size;
            return value;
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer using fixed-width encoding (4 bytes).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadUInt32Fixed()
        {
            const int size = 4;
            if (_position + size > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {size} bytes but only {_buffer.Length - _position} bytes available.");

            uint value =
                (uint)_buffer[_position] |
                (uint)_buffer[_position + 1] << 8 |
                (uint)_buffer[_position + 2] << 16 |
                (uint)_buffer[_position + 3] << 24;
            _position += size;
            return value;
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer using varint or fixed encoding based on formatter options.
        /// </summary>
        /// <returns>The read value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadUInt32Optimized()
        {
            if ((_options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                return ReadVarint32();
            }
            else
            {
                return ReadUInt32Fixed();
            }
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer using fixed-width encoding (8 bytes).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong ReadUInt64Fixed()
        {
            const int size = 8;
            if (_position + size > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {size} bytes but only {_buffer.Length - _position} bytes available.");

            ulong low =
                (ulong)_buffer[_position] |
                (ulong)_buffer[_position + 1] << 8 |
                (ulong)_buffer[_position + 2] << 16 |
                (ulong)_buffer[_position + 3] << 24;
            ulong high =
                (ulong)_buffer[_position + 4] |
                (ulong)_buffer[_position + 5] << 8 |
                (ulong)_buffer[_position + 6] << 16 |
                (ulong)_buffer[_position + 7] << 24;
            _position += size;
            return low | (high << 32);
        }

        /// <summary>
        /// Reads a 32-bit float from the buffer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float ReadSingle()
        {
            const int size = sizeof(float);
            if (_position + size > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {size} bytes but only {_buffer.Length - _position} bytes available.");

            // Use unsafe for direct conversion from bytes
            unsafe
            {
                uint value =
                    (uint)_buffer[_position] |
                    (uint)_buffer[_position + 1] << 8 |
                    (uint)_buffer[_position + 2] << 16 |
                    (uint)_buffer[_position + 3] << 24;
                _position += size;
                return *(float*)&value;
            }
        }

        /// <summary>
        /// Reads a 64-bit double from the buffer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double ReadDouble()
        {
            const int size = sizeof(double);
            if (_position + size > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {size} bytes but only {_buffer.Length - _position} bytes available.");

            // Use unsafe for direct conversion from bytes
            unsafe
            {
                ulong low =
                    (ulong)_buffer[_position] |
                    (ulong)_buffer[_position + 1] << 8 |
                    (ulong)_buffer[_position + 2] << 16 |
                    (ulong)_buffer[_position + 3] << 24;
                ulong high =
                    (ulong)_buffer[_position + 4] |
                    (ulong)_buffer[_position + 5] << 8 |
                    (ulong)_buffer[_position + 6] << 16 |
                    (ulong)_buffer[_position + 7] << 24;
                _position += size;
                ulong value = low | (high << 32);
                return *(double*)&value;
            }
        }

        /// <summary>
        /// Reads a decimal value from the buffer.
        /// Deserializes the decimal from four 32-bit integers (lo, mid, hi, flags).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private decimal ReadDecimal()
        {
            const int size = 4 * sizeof(int); // 4 ints = 16 bytes
            if (_position + size > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {size} bytes but only {_buffer.Length - _position} bytes available.");

            var bits = new int[4];
            for (int i = 0; i < 4; i++)
            {
                bits[i] = (int)ReadUInt32Fixed();
            }

            return new decimal(bits);
        }

        /// <summary>
        /// Reads a primitive array from the buffer using direct memory copy.
        /// </summary>
        /// <typeparam name="T">The primitive type (sbyte, short, int, long, ushort, uint, ulong).</typeparam>
        /// <param name="length">The number of elements to read.</param>
        /// <returns>The primitive array read from the buffer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe T[] ReadPrimitiveArray<T>(int length) where T : unmanaged
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");

            if (length == 0)
                return Array.Empty<T>();

            var result = new T[length];
            int elementSize = sizeof(T);
            int byteCount = length * elementSize;

            if (_position + byteCount > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {byteCount} bytes but only {_buffer.Length - _position} bytes available.");

            fixed (T* resultPtr = result)
            fixed (byte* srcPtr = &_buffer[_position])
            {
                MemoryUtility.FastMemoryCopy(srcPtr, resultPtr, byteCount);
            }

            _position += byteCount;
            return result;
        }

        /// <summary>
        /// Reads an unmanaged value from the buffer using direct memory copy.
        /// </summary>
        /// <typeparam name="T">The unmanaged type to read.</typeparam>
        /// <returns>The value read from the buffer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe T ReadPrimitiveValue<T>() where T : unmanaged
        {
            int byteCount = sizeof(T);
            if (_position + byteCount > _buffer.Length)
                throw new EndOfStreamException(
                    $"Attempted to read {byteCount} bytes but only {_buffer.Length - _position} bytes available.");

            T result;
            fixed (byte* srcPtr = &_buffer[_position])
            {
                T* resultPtr = &result;
                MemoryUtility.FastMemoryCopy(srcPtr, resultPtr, byteCount);
            }

            _position += byteCount;
            return result;
        }

        /// <summary>
        /// Reads and validates a tag when IncludeTypeTags option is enabled.
        /// </summary>
        /// <param name="expectedTag">The expected tag value.</param>
        /// <param name="context">Context description for error messages.</param>
        private void ReadAndValidateOptionTag(BinaryFormatterTag expectedTag, string context)
        {
            if ((_options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                var tag = ReadTag();
                if (tag != expectedTag)
                {
                    throw new DataFormatException(
                        $"Invalid type tag for {context}. Expected {expectedTag}, found {tag}.");
                }
            }
        }

        private BinaryFormatterTag ReadTag()
        {
            return (BinaryFormatterTag)ReadByte();
        }

        /// <summary>
        /// Reads and validates the node depth from the stream.
        /// </summary>
        /// <param name="context">The context description for error messages (e.g., "BeginObject", "EndArray").</param>
        /// <exception cref="DataFormatException">Thrown when the depth value does not match the expected node depth.</exception>
        private void ReadAndValidateNodeDepth(string context)
        {
            var depth = (int)ReadUInt32Optimized();
            if (depth != _nodeDepth)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"Depth mismatch at {context}. Expected {_nodeDepth}, found {depth}.");

                var tag = BitConverter.GetBytes(depth)[3];
                if (Enum.IsDefined(typeof(BinaryFormatterTag), tag))
                {
                    stringBuilder.Append($"(possible tag: BinaryFormatterTag.{(BinaryFormatterTag)tag})");
                }
                throw new DataFormatException(stringBuilder.ToString());
            }
        }

        private Type ReadAndValidateType(Type expectedType, bool enableValidate = true)
        {
            if ((_options & BinaryFormatterOptions.IncludeObjectType) != 0)
            {
                var typeTag = ReadTag();
                Type foundType = null;
                switch (typeTag)
                {
                    case BinaryFormatterTag.NullType:
                        if (enableValidate && expectedType != null)
                        {
                            throw new DataFormatException(
                                $"Type mismatch in binary data. Expected type '{expectedType}', found null.");
                        }

                        break;
                    case BinaryFormatterTag.TypeId:
                    {
                        var typeId = (int)ReadUInt32Optimized();
                        if (!_typeById.TryGetValue(typeId, out foundType))
                        {
                            throw new DataFormatException(
                                $"Type ID {typeId} not found in type dictionary.");
                        }

                        if (enableValidate && !foundType.IsDerivedFrom(expectedType))
                        {
                            throw new DataFormatException(
                                $"Type mismatch in binary data. Expected type '{expectedType}', found '{foundType}'.");
                        }

                        break;
                    }
                    case BinaryFormatterTag.TypeName:
                    {
                        var typeNameLength = ReadUInt32Optimized();
                        var typeName = ReadString((int)typeNameLength);
                        foundType = SerializedTypeUtility.NameToType(typeName);
                        if (enableValidate && !foundType.IsDerivedFrom(expectedType))
                        {
                            throw new DataFormatException(
                                $"Type mismatch in binary data. Expected type '{expectedType}', found '{foundType}'.");
                        }

                        _typeById[_typeById.Count] = foundType;
                        break;
                    }
                    default:
                        throw new DataFormatException(
                            $"Invalid type tag: {typeTag}. Expected NullType, TypeId, or TypeName.");
                }

                return foundType;
            }

            return null;
        }
    }
}
