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
        public JsonReadingFormatter(Stream stream)
        {
            throw new NotImplementedException("JSON reading formatter is not yet implemented.");
        }

        /// <inheritdoc />
        public override SerializationFormat Type => SerializationFormat.Json;

        /// <inheritdoc />
        public override void BeginMember(string name) => throw new NotImplementedException();

        /// <inheritdoc />
        protected override void BeginObject() => throw new NotImplementedException();

        /// <inheritdoc />
        protected override void EndObject() => throw new NotImplementedException();

        /// <inheritdoc />
        protected override void BeginArray(ref int length) => throw new NotImplementedException();

        /// <inheritdoc />
        protected override void EndArray() => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref int value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref sbyte value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref short value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref long value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref byte value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref ushort value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref uint value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref ulong value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref bool value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref float value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref double value) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref string str) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref byte[] data) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void Format(ref UnityEngine.Object unityObject) => throw new NotImplementedException();
    }
}
