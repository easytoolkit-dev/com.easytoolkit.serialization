using System;
using System.IO;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Main entry point for serialization and deserialization operations.
    /// </summary>
    public static class EasySerialize
    {
        /// <summary>Serializes a value to the specified serialization data.</summary>
        /// <typeparam name="T">The type of value to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <param name="serializationData">The serialization data to populate.</param>
        public static void To<T>(T value, ref EasySerializationData serializationData)
        {
            To(value, typeof(T), ref serializationData);
        }

        /// <summary>Deserializes a value from the specified serialization data.</summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="serializationData">The serialization data to read from.</param>
        /// <returns>The deserialized value.</returns>
        public static T From<T>(ref EasySerializationData serializationData)
        {
            return (T)From(typeof(T), ref serializationData);
        }

        /// <summary>Serializes a value to the specified serialization data.</summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="valueType">The type of value to serialize.</param>
        /// <param name="serializationData">The serialization data to populate.</param>
        public static void To(object value, Type valueType, ref EasySerializationData serializationData)
        {
            if (value == null)
            {
                // Debug.LogWarning("Serialize null value!");
                serializationData.SetData(new byte[] { });
                return;
            }

            // Build node tree first
            var nodeBuilder = SerializationGlobalContext.Instance.GetService<ISerializationNodeBuilder>();
            var rootNode = nodeBuilder.BuildNode(valueType);

            using (var stream = new MemoryStream())
            {
                var formatter = SerializationGlobalContext.Instance.GetService<IFormatterFactory>()
                    .CreateWriter(serializationData.FormatterType, stream);

                rootNode.Serializer.Process(ref value, formatter);

                serializationData.ReferencedUnityObjects =
                    new System.Collections.Generic.List<UnityEngine.Object>(formatter.GetObjectTable());
                serializationData.SetData(stream.ToArray());
            }
        }

        /// <summary>Deserializes a value from the specified serialization data.</summary>
        /// <param name="type">The type of value to deserialize.</param>
        /// <param name="serializationData">The serialization data to read from.</param>
        /// <returns>The deserialized value.</returns>
        public static object From(Type type, ref EasySerializationData serializationData)
        {
            // Build node tree first
            var nodeBuilder = SerializationGlobalContext.Instance.GetService<ISerializationNodeBuilder>();
            var rootNode = nodeBuilder.BuildNode(type);

            object res = null;
            var buf = serializationData.GetData();
            if (buf.Length == 0)
                return null;

            using (var stream = new MemoryStream(buf))
            {
                var formatter = SerializationGlobalContext.Instance.GetService<IFormatterFactory>()
                    .CreateReader(serializationData.FormatterType, stream);
                formatter.SetObjectTable(serializationData.ReferencedUnityObjects);
                rootNode.Serializer.Process(ref res, formatter);
            }

            return res;
        }
    }
}
