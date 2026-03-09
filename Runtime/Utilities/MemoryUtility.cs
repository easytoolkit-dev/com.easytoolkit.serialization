using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EasyToolkit.Serialization.Utilities
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
        [StructLayout(LayoutKind.Sequential)]
        private struct Aligned256BitBlock
        {
            public ulong Value1;
            public ulong Value2;
            public ulong Value3;
            public ulong Value4;
        }

        // Maximum safe copy size to prevent potential memory corruption (256 MB)
        private const int MaxSafeCopySize = 256 * 1024 * 1024;

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
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="source"/> or <paramref name="destination"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="bytesToCopy"/> is negative or exceeds the maximum safe copy size.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when source and destination memory regions overlap, which can cause undefined behavior.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastMemoryCopy(void* source, void* destination, int bytesToCopy)
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
                throw new ArgumentOutOfRangeException(nameof(bytesToCopy),
                    "Number of bytes to copy cannot be negative.");
            }

            if (bytesToCopy > MaxSafeCopySize)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesToCopy),
                    $"Number of bytes to copy ({bytesToCopy}) exceeds maximum safe size ({MaxSafeCopySize}). " +
                    "This may indicate a corrupted size value. If you need to copy larger data, " +
                    "copy it in chunks.");
            }

            if (bytesToCopy == 0)
                return;

            // Check for memory overlap before proceeding
            CheckMemoryOverlap(source, destination, bytesToCopy);

            BulkCopyAligned(source, destination, bytesToCopy);
        }

        /// <summary>
        /// Checks if source and destination memory regions overlap.
        /// </summary>
        /// <param name="source">Pointer to source memory.</param>
        /// <param name="destination">Pointer to destination memory.</param>
        /// <param name="bytesToCopy">Number of bytes to copy.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when source and destination memory regions overlap.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckMemoryOverlap(void* source, void* destination, int bytesToCopy)
        {
            var src = (byte*)source;
            var dst = (byte*)destination;

            // Check for overlap without potential integer overflow
            // Region 1: [src, src + bytesToCopy)
            // Region 2: [dst, dst + bytesToCopy)
            // Overlap exists if these ranges intersect

            if (src < dst)
            {
                // Source is before destination - check if source extends into destination
                // Use subtraction to avoid overflow
                var offset = dst - src;
                if (offset < bytesToCopy)
                {
                    throw new ArgumentException(
                        $"Source and destination memory regions overlap. " +
                        $"Source: 0x{(IntPtr)src:X8}, Destination: 0x{(IntPtr)dst:X8}, Size: {bytesToCopy} bytes. " +
                        "This can cause undefined behavior and crashes. Use Buffer.MemoryCopy with proper " +
                        "temp buffer for overlapping regions.",
                        nameof(destination));
                }
            }
            else if (dst < src)
            {
                // Destination is before source - check if destination extends into source
                var offset = src - dst;
                if (offset < bytesToCopy)
                {
                    throw new ArgumentException(
                        $"Source and destination memory regions overlap. " +
                        $"Source: 0x{(IntPtr)src:X8}, Destination: 0x{(IntPtr)dst:X8}, Size: {bytesToCopy} bytes. " +
                        "This can cause undefined behavior and crashes. Use Buffer.MemoryCopy with proper " +
                        "temp buffer for overlapping regions.",
                        nameof(destination));
                }
            }
            // If src == dst, pointers are identical (harmless but useless)
        }

        /// <summary>
        /// Performs 256-bit aligned bulk memory copy.
        /// Optimized for modern CPU architectures with 256-bit registers.
        /// </summary>
        /// <param name="source">Pointer to source memory.</param>
        /// <param name="destination">Pointer to destination memory.</param>
        /// <param name="bytesToCopy">Number of bytes to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BulkCopyAligned(void* source, void* destination, int bytesToCopy)
        {
            byte* src = (byte*)source;
            byte* dst = (byte*)destination;

            // Calculate end pointer safely, avoiding potential overflow
            // Use checked arithmetic to catch overflow conditions
            byte* end;
            try
            {
                end = dst + bytesToCopy;
            }
            catch (OverflowException)
            {
                throw new OverflowException(
                    $"Pointer arithmetic overflow detected. Destination pointer (0x{(IntPtr)dst:X8}) " +
                    $"plus size ({bytesToCopy}) would overflow. This may indicate corrupted size data.");
            }

            // Copy 256-bit (32-byte) aligned blocks
            if (bytesToCopy >= sizeof(Aligned256BitBlock))
            {
                // Verify alignment before casting to struct pointer
                // Misaligned access can cause crashes on some architectures
                var srcAlignment = (nuint)src & (nuint)(sizeof(Aligned256BitBlock) - 1);
                var dstAlignment = (nuint)dst & (nuint)(sizeof(Aligned256BitBlock) - 1);

                // Only use aligned copy if both pointers are properly aligned
                if (srcAlignment == 0 && dstAlignment == 0)
                {
                    Aligned256BitBlock* srcBlock = (Aligned256BitBlock*)src;
                    Aligned256BitBlock* dstBlock = (Aligned256BitBlock*)dst;

                    int blockCount = bytesToCopy / sizeof(Aligned256BitBlock);
                    for (int i = 0; i < blockCount; i++)
                    {
                        // Validate pointers before dereferencing
                        if (srcBlock == null || dstBlock == null)
                        {
                            throw new InvalidOperationException(
                                "Invalid pointer state detected during bulk copy operation. " +
                                "This may indicate memory corruption.");
                        }

                        *dstBlock++ = *srcBlock++;
                    }

                    src = (byte*)srcBlock;
                    dst = (byte*)dstBlock;
                }
                else
                {
                    // Fall back to byte-by-byte copy for misaligned data
                    // This is slower but safe
                    int remaining = bytesToCopy;
                    while (remaining-- > 0)
                    {
                        *dst++ = *src++;
                    }
                    return;
                }
            }

            // Copy remaining bytes
            while (dst < end)
            {
                *dst++ = *src++;
            }
        }
    }
}
