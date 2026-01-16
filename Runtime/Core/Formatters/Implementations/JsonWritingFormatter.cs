using System;
using System.IO;
using UnityEngine;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// JSON writing formatter implementation (not yet implemented).
    /// </summary>
    public sealed class JsonWritingFormatter : WritingFormatterBase
    {
        /// <summary>Creates a new JSON writing formatter.</summary>
        /// <param name="stream">The stream to write to.</param>
        public JsonWritingFormatter(Stream stream)
        {
            throw new NotImplementedException("JSON writing formatter is not yet implemented.");
        }

        /// <inheritdoc />
        public override SerializationFormat Type => SerializationFormat.Json;

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
