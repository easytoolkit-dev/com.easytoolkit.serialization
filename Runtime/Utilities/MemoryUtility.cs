using System;
using System.Runtime.CompilerServices;

namespace EasyToolKit.Serialization.Utilities
{
/// <summary>
/// Provides high-performance memory manipulation utilities with endianness support for serialization.
/// This utility assumes all source data is stored in little-endian format and automatically handles
/// endianness conversion based on the current system architecture.
/// </summary>
/// <remarks>
/// This class is designed for serialization libraries where data is stored in a consistent
/// endianness (little-endian) and needs to be properly interpreted on both little-endian
/// and big-endian systems.
/// </remarks>
public static unsafe class MemoryUtility
{
    // Structure for 256-bit (32-byte) aligned memory transfers
    private struct Aligned256BitBlock
    {
        public ulong Value1;
        public ulong Value2;
        public ulong Value3;
        public ulong Value4;
    }

    /// <summary>
    /// Indicates whether the current system architecture is little-endian.
    /// This is used to determine if byte order conversion is required when reading
    /// from little-endian formatted data.
    /// </summary>
    private static readonly bool IsCurrentSystemLittleEndian = BitConverter.IsLittleEndian;

    /// <summary>
    /// Copies memory from a little-endian source to the current system memory,
    /// automatically performing byte order conversion if the current system is big-endian.
    /// </summary>
    /// <param name="littleEndianSource">Pointer to source memory containing little-endian formatted data.</param>
    /// <param name="destination">Pointer to destination memory in current system's native byte order.</param>
    /// <param name="bytesToCopy">Number of bytes to copy.</param>
    /// <remarks>
    /// This method assumes source data is stored in little-endian format (the serialization standard).
    /// On little-endian systems, a direct optimized copy is performed.
    /// On big-endian systems, byte order conversion is automatically applied.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyFromLittleEndian(
        void* littleEndianSource,
        void* destination,
        int bytesToCopy)
    {
        ValidateArguments(littleEndianSource, destination, bytesToCopy);

        if (bytesToCopy == 0)
            return;

        // On little-endian systems, we can copy directly
        if (IsCurrentSystemLittleEndian)
        {
            FastMemoryCopy(littleEndianSource, destination, bytesToCopy);
            return;
        }

        // On big-endian systems, we need to reverse byte order
        ReverseByteOrder(littleEndianSource, destination, bytesToCopy);
    }

    /// <summary>
    /// Copies memory from the current system memory to a little-endian destination,
    /// automatically performing byte order conversion if the current system is big-endian.
    /// </summary>
    /// <param name="source">Pointer to source memory in current system's native byte order.</param>
    /// <param name="littleEndianDestination">Pointer to destination memory that will store little-endian formatted data.</param>
    /// <param name="bytesToCopy">Number of bytes to copy.</param>
    /// <remarks>
    /// This method stores data in little-endian format (the serialization standard).
    /// On little-endian systems, a direct optimized copy is performed.
    /// On big-endian systems, byte order conversion is automatically applied.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyToLittleEndian(
        void* source,
        void* littleEndianDestination,
        int bytesToCopy)
    {
        ValidateArguments(source, littleEndianDestination, bytesToCopy);

        if (bytesToCopy == 0)
            return;

        // On little-endian systems, we can copy directly
        if (IsCurrentSystemLittleEndian)
        {
            FastMemoryCopy(source, littleEndianDestination, bytesToCopy);
            return;
        }

        // On big-endian systems, we need to reverse byte order
        ReverseByteOrder(source, littleEndianDestination, bytesToCopy);
    }

    /// <summary>
    /// Performs a fast memory copy assuming same byte order between source and destination.
    /// Uses 256-bit (32-byte) aligned transfers for optimal performance.
    /// </summary>
    /// <param name="source">Pointer to source memory.</param>
    /// <param name="destination">Pointer to destination memory.</param>
    /// <param name="bytesToCopy">Number of bytes to copy.</param>
    /// <remarks>
    /// This method performs a direct memory copy without endianness conversion.
    /// Use this when you know both source and destination use the same byte order,
    /// or when working with raw byte data that doesn't represent numeric types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FastMemoryCopy(void* source, void* destination, int bytesToCopy)
    {
        ValidateArguments(source, destination, bytesToCopy);

        if (bytesToCopy == 0)
            return;

        BulkCopyAligned(source, destination, bytesToCopy);
    }

    /// <summary>
    /// Reverses the byte order of memory during copy operation.
    /// This is used when copying between little-endian and big-endian systems.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReverseByteOrder(void* source, void* destination, int bytesToCopy)
    {
        byte* src = (byte*)source;
        byte* dst = (byte*)destination;
        byte* endSrc = src + bytesToCopy;

        // Process 64-bit (8-byte) chunks for efficient byte order reversal
        if (bytesToCopy >= sizeof(ulong))
        {
            ulong* src64 = (ulong*)src;
            ulong* dst64 = (ulong*)dst;
            int chunkCount = bytesToCopy / sizeof(ulong);

            for (int i = 0; i < chunkCount; i++)
            {
                *dst64++ = ReverseUInt64(*src64++);
            }

            src = (byte*)src64;
            dst = (byte*)dst64;
        }

        // Process remaining bytes (less than 8)
        int remainingBytes = bytesToCopy % sizeof(ulong);
        if (remainingBytes > 0)
        {
            // Reverse byte order for the remaining bytes
            for (int i = 0; i < remainingBytes; i++)
            {
                dst[remainingBytes - 1 - i] = src[i];
            }
        }
    }

    /// <summary>
    /// Performs 256-bit aligned bulk memory copy.
    /// Optimized for modern CPU architectures with 256-bit registers.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void BulkCopyAligned(void* source, void* destination, int bytesToCopy)
    {
        byte* src = (byte*)source;
        byte* dst = (byte*)destination;
        byte* end = dst + bytesToCopy;

        // Copy 256-bit (32-byte) aligned blocks
        if (bytesToCopy >= sizeof(Aligned256BitBlock))
        {
            Aligned256BitBlock* srcBlock = (Aligned256BitBlock*)src;
            Aligned256BitBlock* dstBlock = (Aligned256BitBlock*)dst;

            int blockCount = bytesToCopy / sizeof(Aligned256BitBlock);
            for (int i = 0; i < blockCount; i++)
            {
                *dstBlock++ = *srcBlock++;
            }

            src = (byte*)srcBlock;
            dst = (byte*)dstBlock;
        }

        // Copy remaining bytes
        while (dst < end)
        {
            *dst++ = *src++;
        }
    }

    /// <summary>
    /// Reverses the byte order of a 64-bit unsigned integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ReverseUInt64(ulong value)
    {
        // Swap bytes using bitwise operations
        return ((value & 0x00000000000000FFUL) << 56) |
               ((value & 0x000000000000FF00UL) << 40) |
               ((value & 0x0000000000FF0000UL) << 24) |
               ((value & 0x00000000FF000000UL) << 8)  |
               ((value & 0x000000FF00000000UL) >> 8)  |
               ((value & 0x0000FF0000000000UL) >> 24) |
               ((value & 0x00FF000000000000UL) >> 40) |
               ((value & 0xFF00000000000000UL) >> 56);
    }

    /// <summary>
    /// Validates method arguments and throws appropriate exceptions.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateArguments(void* source, void* destination, int bytesToCopy)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source), "Source pointer cannot be null.");
        }

        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination), "Destination pointer cannot be null.");
        }

        if (bytesToCopy < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytesToCopy), "Number of bytes to copy cannot be negative.");
        }
    }
}
}
