using System;
using System.IO;
using UnityEngine;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// JSON reading formatter implementation (not yet implemented).
    /// </summary>
    public sealed class JsonReadingFormatter : ReadingFormatterBase
    {
        /// <summary>Creates a new JSON reading formatter.</summary>
        /// <param name="stream">The stream to read from.</param>
        public JsonReadingFormatter(Stream stream)
        {
            throw new NotImplementedException("JSON reading formatter is not yet implemented.");
        }

        /// <inheritdoc />
        public override FormatterType Type => FormatterType.Json;

        /// <inheritdoc />
        public override void BeginMember(string name) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void EndMember() => throw new NotImplementedException();

        /// <inheritdoc />
        public override void BeginObject() => throw new NotImplementedException();

        /// <inheritdoc />
        public override void EndObject() => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool Format(ref int value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool Format(ref Varint32 value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool Format(ref SizeTag size) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool Format(ref bool value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool Format(ref float value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool Format(ref double value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool Format(ref string str) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool Format(ref byte[] data) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool Format(ref UnityEngine.Object unityObject) => throw new NotImplementedException();
    }
}
