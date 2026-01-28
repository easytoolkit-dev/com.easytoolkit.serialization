using System;
using System.Runtime.CompilerServices;

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
        private struct Aligned256BitBlock
        {
            public ulong Value1;
            public ulong Value2;
            public ulong Value3;
            public ulong Value4;
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

            if (bytesToCopy == 0)
                return;

            BulkCopyAligned(source, destination, bytesToCopy);
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
    }
}
