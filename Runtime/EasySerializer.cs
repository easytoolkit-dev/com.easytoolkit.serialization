using System;
using System.IO;

namespace EasyToolKit.Serialization
{
    public static class EasySerializer
    {
        /// <summary>Serializes a value to the specified serialization data.</summary>
        /// <typeparam name="T">The type of value to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <param name="serializationData">The serialization data to populate.</param>
        public static void Serialize<T>(T value, ref EasySerializationData serializationData)
        {
            if (value == null)
            {
                // Debug.LogWarning("Serialize null value!");
                serializationData.SetData(new byte[] { });
                return;
            }

            // Build node tree first
            using var stream = new MemoryStream();
            var formatter = SerializationEnvironment.Instance.GetFactory<IFormatterFactory>()
                .CreateWriter(serializationData.FormatterType, stream);

            var serializer = SerializationEnvironment.Instance.GetFactory<ISerializationProcessorFactory>()
                .GetSerializer<T>();
            serializer.Process(ref value, formatter);

            serializationData.ReferencedUnityObjects =
                new System.Collections.Generic.List<UnityEngine.Object>(formatter.GetObjectTable());
            serializationData.SetData(stream.ToArray());
        }

        /// <summary>Deserializes a value from the specified serialization data.</summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="serializationData">The serialization data to read from.</param>
        /// <returns>The deserialized value.</returns>
        public static T Deserialize<T>(ref EasySerializationData serializationData)
        {
            T result = default;
            var buf = serializationData.GetData();
            if (buf.Length == 0)
                return default;

            using var stream = new MemoryStream(buf);
            var formatter = SerializationEnvironment.Instance.GetFactory<IFormatterFactory>()
                .CreateReader(serializationData.FormatterType, stream);
            formatter.SetObjectTable(serializationData.ReferencedUnityObjects);

            var serializer = SerializationEnvironment.Instance.GetFactory<ISerializationProcessorFactory>()
                .GetSerializer<T>();
            serializer.Process(ref result, formatter);

            return result;
        }
    }
}
