using System;
using System.IO;
using System.Runtime.CompilerServices;
using EasyToolKit.Serialization.Utilities;

namespace EasyToolKit.Serialization.Formatters.Implementations
{
    public sealed partial class BinaryReadingFormatter
    {
        /// <summary>Reads a string from raw char memory (2 bytes per char) with the specified char count.</summary>
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

        /// <summary>Reads a single byte from the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte ReadByte()
        {
            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");
            return _buffer[_position++];
        }

        /// <summary>Reads the specified number of bytes from the buffer.</summary>
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

        /// <summary>Reads a 32-bit unsigned integer using variable-length decoding.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadVarint32()
        {
            uint value = 0;
            int shift = 0;

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

        /// <summary>Reads a 64-bit unsigned integer using variable-length decoding.</summary>
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

        /// <summary>Reads a 32-bit float from the buffer.</summary>
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

        /// <summary>Reads a 64-bit double from the buffer.</summary>
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
    }
}
