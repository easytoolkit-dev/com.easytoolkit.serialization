using System;
using System.Collections.Generic;
using System.IO;

namespace EasyToolKit.Serialization
{
    public static class EasySerializer
    {
        public static void Serialize<T>(ref T value, SerializationFormat format, ref SerializationData serializationData)
        {
            if (value == null)
            {
                serializationData.Clear();
                return;
            }

            // Build node tree first
            using var stream = new MemoryStream();
            var formatter = SerializationEnvironment.Instance.GetFactory<IFormatterFactory>()
                .CreateWriter(format, stream);
            if (formatter == null)
            {
                throw new SerializationException(
                    $"Failed to create writer for format '{format}'. The format may not be supported.");
            }

            var processor = SerializationEnvironment.Instance.GetFactory<ISerializationProcessorFactory>()
                .GetProcessor<T>();
            if (processor == null)
            {
                throw new SerializationException(
                    $"Cannot serialize type '{typeof(T)}'. No serialization processor found for this type. " +
                    $"Ensure the type is either a primitive type, a collection, or marked with [Serializable] or [EasySerializable].");
            }

            processor.Process(ref value, formatter);

            var objectTable = formatter.GetObjectTable();
            if (objectTable.Count > 0)
            {
                serializationData.ReferencedUnityObjects = new List<UnityEngine.Object>(objectTable);
            }
            else
            {
                serializationData.ReferencedUnityObjects = null;
            }
            serializationData.SetBuffer(format, stream.ToArray());
        }

        public static byte[] SerializeToBinary<T>(ref T value)
        {
            List<UnityEngine.Object> referencedUnityObjects = null;
            return SerializeToBinary(ref value, ref referencedUnityObjects);
        }

        public static byte[] SerializeToBinary<T>(ref T value, ref List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(Array.Empty<byte>(), referencedUnityObjects);
            Serialize(ref value, SerializationFormat.Binary, ref serializationData);
            referencedUnityObjects = serializationData.ReferencedUnityObjects;
            return serializationData.BinaryData;
        }

        public static T Deserialize<T>(SerializationFormat format, ref SerializationData serializationData)
        {
            var buffer = serializationData.GetBuffer(format);
            if (buffer == null)
            {
                return default;
            }

            T result = default;
            using var stream = new MemoryStream(buffer);
            var formatter = SerializationEnvironment.Instance.GetFactory<IFormatterFactory>()
                .CreateReader(format, stream);
            if (formatter == null)
            {
                throw new SerializationException(
                    $"Failed to create reader for format '{format}'. The format may not be supported.");
            }

            formatter.SetObjectTable(serializationData.ReferencedUnityObjects);

            var processor = SerializationEnvironment.Instance.GetFactory<ISerializationProcessorFactory>()
                .GetProcessor<T>();
            if (processor == null)
            {
                throw new SerializationException(
                    $"Cannot deserialize type '{typeof(T)}'. No serialization processor found for this type. " +
                    $"Ensure the type is either a primitive type, a collection, or marked with [Serializable] or [EasySerializable].");
            }

            processor.Process(ref result, formatter);

            return result;
        }

        public static T DeserializeFromBinary<T>(byte[] data)
        {
            return DeserializeFromBinary<T>(data, null);
        }

        public static T DeserializeFromBinary<T>(byte[] data, List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(data, referencedUnityObjects);
            return Deserialize<T>(SerializationFormat.Binary, ref serializationData);
        }
    }
}
